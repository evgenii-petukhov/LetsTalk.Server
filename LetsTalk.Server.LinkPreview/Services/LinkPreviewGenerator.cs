using LetsTalk.Server.LinkPreview.Abstractions;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Web;

namespace LetsTalk.Server.LinkPreview.Services;

public class LinkPreviewGenerator : ILinkPreviewGenerator
{
    private readonly ILinkPreviewRepository _linkPreviewRepository;
    private readonly IDownloadService _downloadService;
    private readonly IRegexService _regexService;
    private readonly ILogger<LinkPreviewGenerator> _logger;
    private readonly IEntityFactory _entityFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageDomainService _messageDomainService;

    public LinkPreviewGenerator(
        ILinkPreviewRepository linkPreviewRepository,
        IDownloadService downloadService,
        IRegexService regexService,
        ILogger<LinkPreviewGenerator> logger,
        IEntityFactory entityFactory,
        IUnitOfWork unitOfWork,
        IMessageDomainService messageDomainService)
    {
        _linkPreviewRepository = linkPreviewRepository;
        _downloadService = downloadService;
        _regexService = regexService;
        _logger = logger;
        _entityFactory = entityFactory;
        _unitOfWork = unitOfWork;
        _messageDomainService = messageDomainService;
    }

    public async Task SetLinkPreviewAsync(int messageId, string url)
    {
        var linkPreview = await _linkPreviewRepository.GetByUrlOrDefaultAsync(url);
        if (linkPreview == null)
        {
            try
            {
                var pageString = await _downloadService.DownloadAsStringAsync(url);

                var (title, imageUrl) = _regexService.GetOpenGraphModel(pageString);

                if (string.IsNullOrWhiteSpace(title))
                {
                    _logger.LogInformation("Title is empty: {url}", url);
                    return;
                }

                title = HttpUtility.HtmlDecode(title);
                try
                {
                    linkPreview = _entityFactory.CreateLinkPreview(url, title, imageUrl!);
                    _logger.LogInformation("New LinkPreview added: {@linkPreview}", linkPreview);
                    await _messageDomainService.SetLinkPreviewAsync(linkPreview, messageId);
                    await _unitOfWork.SaveAsync();

                    return;
                }
                catch (DbUpdateException)
                {
                    linkPreview = await _linkPreviewRepository.GetByUrlOrDefaultAsync(url);
                    _logger.LogInformation("Fetched from DB: {@linkPreview}", linkPreview);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to download: {url}", url);
                return;
            }
        }
        else
        {
            _logger.LogInformation("Fetched from DB: {@linkPreview}", linkPreview);
        }

        await _messageDomainService.SetLinkPreviewAsync(linkPreview!, messageId);
        await _unitOfWork.SaveAsync();
    }
}

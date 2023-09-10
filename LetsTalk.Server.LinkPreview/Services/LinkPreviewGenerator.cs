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

    public LinkPreviewGenerator(
        ILinkPreviewRepository linkPreviewRepository,
        IDownloadService downloadService,
        IRegexService regexService,
        ILogger<LinkPreviewGenerator> logger,
        IEntityFactory entityFactory)
    {
        _linkPreviewRepository = linkPreviewRepository;
        _downloadService = downloadService;
        _regexService = regexService;
        _logger = logger;
        _entityFactory = entityFactory;
    }

    public async Task<Domain.LinkPreview?> GetLinkPreviewAsync(string url)
    {
        var linkPreview = await _linkPreviewRepository.GetByUrlOrDefaultAsync(url);
        if (linkPreview == null)
        {
            try
            {
                var pageString = await _downloadService.DownloadAsStringAsync(url);

                var openGraphModel = _regexService.GetOpenGraphModel(pageString);

                if (string.IsNullOrWhiteSpace(openGraphModel.Title))
                {
                    _logger.LogInformation("Title is empty: {url}", url);
                    return null;
                }

                openGraphModel.Title = HttpUtility.HtmlDecode(openGraphModel.Title);
                try
                {
                    linkPreview = _entityFactory.CreateLinkPreview(url, openGraphModel.Title, openGraphModel.ImageUrl!);
                    await _linkPreviewRepository.CreateAsync(linkPreview);
                    _logger.LogInformation("New LinkPreview added: {@linkPreview}", linkPreview);
                }
                catch (DbUpdateException)
                {
                    linkPreview = await _linkPreviewRepository.GetByUrlOrDefaultAsync(url);
                    _logger.LogInformation("Fetched from DB: {@linkPreview}", linkPreview);
                }
                return linkPreview;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to download: {url}", url);
                return null;
            }
        }
        else
        {
            _logger.LogInformation("Fetched from DB: {@linkPreview}", linkPreview);
            return linkPreview;
        }
    }
}

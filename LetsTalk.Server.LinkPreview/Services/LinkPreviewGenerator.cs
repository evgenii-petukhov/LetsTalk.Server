using LetsTalk.Server.LinkPreview.Abstractions;
using LetsTalk.Server.LinkPreview.Models;
using LetsTalk.Server.Persistence.Abstractions;
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

    public LinkPreviewGenerator(
        ILinkPreviewRepository linkPreviewRepository,
        IDownloadService downloadService,
        IRegexService regexService,
        ILogger<LinkPreviewGenerator> logger)
    {
        _linkPreviewRepository = linkPreviewRepository;
        _downloadService = downloadService;
        _regexService = regexService;
        _logger = logger;
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
                    linkPreview = new Domain.LinkPreview
                    {
                        Url = url,
                        Title = openGraphModel.Title,
                        ImageUrl = openGraphModel.ImageUrl
                    };
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

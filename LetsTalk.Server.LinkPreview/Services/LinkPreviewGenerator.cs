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
    private readonly ILogger<LinkPreviewRequest> _logger;

    public LinkPreviewGenerator(
        ILinkPreviewRepository linkPreviewRepository,
        IDownloadService downloadService,
        IRegexService regexService,
        ILogger<LinkPreviewRequest> logger)
    {
        _linkPreviewRepository = linkPreviewRepository;
        _downloadService = downloadService;
        _regexService = regexService;
        _logger = logger;
    }

    public async Task<Domain.LinkPreview?> GetLinkPreviewAsync(string url)
    {
        var linkPreview = await _linkPreviewRepository.GetByUrlAsync(url);

        if (linkPreview != null)
        {
            _logger.LogInformation("Fetched from DB: {url}", url);
            return linkPreview;
        }

        try
        {
            var pageString = await _downloadService.DownloadAsStringAsync(url);

            var openGraphModel = _regexService.GetOpenGraphModel(pageString);
            _logger.LogInformation("{@opModel}", openGraphModel);

            if (string.IsNullOrWhiteSpace(openGraphModel.Title))
            {
                _logger.LogInformation("og:title not found: {url}", url);
                return null;
            }

            openGraphModel.Title = HttpUtility.HtmlDecode(openGraphModel.Title);
            try
            {
                linkPreview = await _linkPreviewRepository.CreateAsync(new Domain.LinkPreview
                {
                    Url = url,
                    Title = openGraphModel.Title,
                    ImageUrl = openGraphModel.ImageUrl
                });
            }
            catch(DbUpdateException)
            {
                linkPreview = await _linkPreviewRepository.GetByUrlAsync(url);
            }

            _logger.LogInformation("{@linkPreview}", linkPreview);
            return linkPreview;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to download: {url}", url);
            return null;
        }
    }
}

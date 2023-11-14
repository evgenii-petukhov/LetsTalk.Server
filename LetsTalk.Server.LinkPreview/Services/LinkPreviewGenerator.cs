using LetsTalk.Server.LinkPreview.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using Microsoft.Extensions.Logging;
using System.Web;

namespace LetsTalk.Server.LinkPreview.Services;

public class LinkPreviewGenerator : ILinkPreviewGenerator
{
    private readonly ILinkPreviewAgnosticService _linkPreviewAgnosticService;
    private readonly IMessageAgnosticService _messageAgnosticService;
    private readonly IDownloadService _downloadService;
    private readonly IRegexService _regexService;
    private readonly ILogger<LinkPreviewGenerator> _logger;

    public LinkPreviewGenerator(
        ILinkPreviewAgnosticService linkPreviewAgnosticService,
        IMessageAgnosticService messageAgnosticService,
        IDownloadService downloadService,
        IRegexService regexService,
        ILogger<LinkPreviewGenerator> logger)
    {
        _linkPreviewAgnosticService = linkPreviewAgnosticService;
        _messageAgnosticService = messageAgnosticService;
        _downloadService = downloadService;
        _regexService = regexService;
        _logger = logger;
    }

    public async Task<MessageServiceModel?> ProcessMessageAsync(int messageId, string url)
    {
        var linkPreviewId = await _linkPreviewAgnosticService.GetIdByUrlAsync(url);
        if (linkPreviewId == 0)
        {
            try
            {
                var pageString = await _downloadService.DownloadAsStringAsync(url);

                var (title, imageUrl) = _regexService.GetOpenGraphModel(pageString);

                if (string.IsNullOrWhiteSpace(title))
                {
                    _logger.LogInformation("Title is empty: {url}", url);
                    return null;
                }

                title = HttpUtility.HtmlDecode(title);
                try
                {
                    var message = await _messageAgnosticService.SetLinkPreviewAsync(messageId, url, title, imageUrl!);
                    _logger.LogInformation("New LinkPreview added: {url}", url);
                    return message;
                }
                catch
                {
                    linkPreviewId = await _linkPreviewAgnosticService.GetIdByUrlAsync(url);
                    _logger.LogInformation("Fetched from DB: {url}", url);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to download: {url}", url);
                return null;
            }
        }
        else
        {
            _logger.LogInformation("Fetched from DB: {url}", url);
        }

        return await _messageAgnosticService.SetLinkPreviewAsync(messageId, linkPreviewId);
    }
}

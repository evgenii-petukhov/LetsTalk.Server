using LetsTalk.Server.LinkPreview.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using Microsoft.Extensions.Logging;
using System.Web;

namespace LetsTalk.Server.LinkPreview.Services;

public class LinkPreviewGenerator(
    ILinkPreviewAgnosticService linkPreviewAgnosticService,
    IMessageAgnosticService messageAgnosticService,
    IDownloadService downloadService,
    IRegexService regexService,
    ILogger<LinkPreviewGenerator> logger) : ILinkPreviewGenerator
{
    private readonly ILinkPreviewAgnosticService _linkPreviewAgnosticService = linkPreviewAgnosticService;
    private readonly IMessageAgnosticService _messageAgnosticService = messageAgnosticService;
    private readonly IDownloadService _downloadService = downloadService;
    private readonly IRegexService _regexService = regexService;
    private readonly ILogger<LinkPreviewGenerator> _logger = logger;

    public async Task<MessageServiceModel?> ProcessMessageAsync(string messageId, string url)
    {
        var linkPreviewId = await _linkPreviewAgnosticService.GetIdByUrlAsync(url);
        if (linkPreviewId == null)
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

        return await _messageAgnosticService.SetLinkPreviewAsync(messageId, linkPreviewId!);
    }
}

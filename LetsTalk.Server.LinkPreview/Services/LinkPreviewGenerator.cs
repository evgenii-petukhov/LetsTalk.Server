using LetsTalk.Server.LinkPreview.Abstractions;
using LetsTalk.Server.LinkPreview.Utility.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace LetsTalk.Server.LinkPreview.Services;

public class LinkPreviewGenerator(
    ILinkPreviewAgnosticService linkPreviewAgnosticService,
    IMessageAgnosticService messageAgnosticService,
    ILinkPreviewService linkPreviewService,
    ILogger<LinkPreviewGenerator> logger) : ILinkPreviewGenerator
{
    private readonly ILinkPreviewAgnosticService _linkPreviewAgnosticService = linkPreviewAgnosticService;
    private readonly IMessageAgnosticService _messageAgnosticService = messageAgnosticService;
    private readonly ILinkPreviewService _linkPreviewService = linkPreviewService;
    private readonly ILogger<LinkPreviewGenerator> _logger = logger;

    public async Task<MessageServiceModel?> ProcessMessageAsync(string messageId, string url)
    {
        var model = await _linkPreviewService.GenerateLinkPreviewAsync(url);

        if (model == null)
        {
            _logger.LogInformation("Title is empty: {url}", url);
            return null;
        }
        else
        {
            if (model.Exception == null)
            {
                try
                {
                    var message = await _messageAgnosticService.SetLinkPreviewAsync(messageId, url, model.OpenGraphModel!.Title!, model.OpenGraphModel!.ImageUrl!);
                    _logger.LogInformation("New LinkPreview added: {url}", url);
                    return message;
                }
                catch
                {
                    var linkPreviewId = await _linkPreviewAgnosticService.GetIdByUrlAsync(url);
                    var message = await _messageAgnosticService.SetLinkPreviewAsync(messageId, linkPreviewId!);
                    _logger.LogInformation("Fetched from DB: {url}", url);
                    return message;
                }
            }
            else
            {
                _logger.LogError(model.Exception, "Unable to download: {url}", url);
                return null;
            }
        }
    }
}

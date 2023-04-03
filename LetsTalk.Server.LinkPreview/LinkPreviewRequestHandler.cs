using KafkaFlow.TypedHandler;
using KafkaFlow;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.LinkPreview.Abstractions;
using Microsoft.Extensions.Logging;

namespace LetsTalk.Server.LinkPreview;

public class LinkPreviewRequestHandler : IMessageHandler<LinkPreviewRequest>
{
    private readonly IDownloadService _downloadService;
    private readonly IRegexService _regexService;
    private readonly ILogger<LinkPreviewRequest> _logger;

    public LinkPreviewRequestHandler(
        IDownloadService downloadService,
        IRegexService regexService,
        ILogger<LinkPreviewRequest> logger)
    {
        _downloadService = downloadService;
        _regexService = regexService;
        _logger = logger;
    }

    public async Task Handle(IMessageContext context, LinkPreviewRequest request)
    {
        if (request.Url == null) return;

        var pageString = await _downloadService.DownloadAsString(request.Url);
        if (string.IsNullOrEmpty(pageString))
        {
            Console.WriteLine("error");
        }
        else
        {
            var m = _regexService.GetOpenGraphModel(pageString);
            _logger.LogInformation("{@m}", m);
        }
    }
}

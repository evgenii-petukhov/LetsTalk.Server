using KafkaFlow.TypedHandler;
using KafkaFlow;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.LinkPreview.Abstractions;

namespace LetsTalk.Server.LinkPreview;

public class LinkPreviewRequestHandler : IMessageHandler<LinkPreviewRequest>
{
    private readonly IDownloadService _downloadService;

    public LinkPreviewRequestHandler(IDownloadService downloadService)
    {
        _downloadService = downloadService;
    }

    public async Task Handle(IMessageContext context, LinkPreviewRequest request)
    {
        if (request.Url != null)
        {
            var pageString = await _downloadService.DownloadAsString(request.Url);
            if (string.IsNullOrEmpty(pageString))
            {
                Console.WriteLine("error");
            }
            else
            {
                Console.WriteLine(pageString);
            }
        }
    }
}

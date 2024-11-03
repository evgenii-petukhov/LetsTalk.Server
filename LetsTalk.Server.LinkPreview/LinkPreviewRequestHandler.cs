using KafkaFlow;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.LinkPreview.Abstractions;

namespace LetsTalk.Server.LinkPreview;

public class LinkPreviewRequestHandler(ILinkPreviewGenerator linkPreviewGenerator) : IMessageHandler<LinkPreviewRequest>
{
    private readonly ILinkPreviewGenerator _linkPreviewGenerator = linkPreviewGenerator;

    public async Task Handle(IMessageContext context, LinkPreviewRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return;
        }

        await _linkPreviewGenerator.ProcessMessageAsync(request.MessageId!, request.ChatId!, request.Url);
    }
}

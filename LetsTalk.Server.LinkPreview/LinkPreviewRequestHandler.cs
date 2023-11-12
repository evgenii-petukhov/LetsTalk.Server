using KafkaFlow;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.LinkPreview.Abstractions;

namespace LetsTalk.Server.LinkPreview;

public class LinkPreviewRequestHandler : IMessageHandler<LinkPreviewRequest>
{
    private readonly ILinkPreviewGenerator _linkPreviewGenerator;

    public LinkPreviewRequestHandler(ILinkPreviewGenerator linkPreviewGenerator)
    {
        _linkPreviewGenerator = linkPreviewGenerator;
    }

    public Task Handle(IMessageContext context, LinkPreviewRequest request)
    {
        return request.Url == null
            ? Task.CompletedTask
            : _linkPreviewGenerator.SetLinkPreviewAsync(request.MessageId, request.Url);
    }
}

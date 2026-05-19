using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Kafka.Models;

namespace LetsTalk.Server.API.Core.Services;

public class LinkPreviewServiceLauncher(
    IProducer<LinkPreviewRequest> linkPreviewProducer) : ILinkPreviewLauncher
{
    private readonly IProducer<LinkPreviewRequest> _linkPreviewProducer = linkPreviewProducer;

    public Task LaunchAsync(
        string messageId,
        string url,
        string chatId,
        string token,
        CancellationToken cancellationToken)
    {
        return _linkPreviewProducer.PublishAsync(new LinkPreviewRequest
        {
            MessageId = messageId,
            Url = url,
            ChatId = chatId,
            Token = token,
        }, cancellationToken);
    }
}

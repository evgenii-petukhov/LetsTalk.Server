using KafkaFlow.TypedHandler;
using KafkaFlow;
using LetsTalk.Server.LinkPreview.Abstractions;
using LetsTalk.Server.Configuration.Models;
using KafkaFlow.Producers;
using Microsoft.Extensions.Options;
using LetsTalk.Server.Notifications.Models;
using LetsTalk.Server.LinkPreview.Models;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.Abstractions;

namespace LetsTalk.Server.LinkPreview;

public class LinkPreviewRequestHandler : IMessageHandler<LinkPreviewRequest>
{
    private readonly IMessageRepository _messageRepository;
    private readonly ILinkPreviewGenerator _linkPreviewGenerator;
    private readonly KafkaSettings _kafkaSettings;

    private readonly IMessageProducer _producer;

    public LinkPreviewRequestHandler(
        IProducerAccessor producerAccessor,
        IOptions<KafkaSettings> kafkaSettings,
        IMessageRepository messageRepository,
        ILinkPreviewGenerator linkPreviewGenerator)
    {
        _kafkaSettings = kafkaSettings.Value;
        _messageRepository = messageRepository;
        _linkPreviewGenerator = linkPreviewGenerator;
        _producer = producerAccessor.GetProducer(_kafkaSettings.LinkPreviewNotification!.Producer);
    }

    public async Task Handle(IMessageContext context, LinkPreviewRequest request)
    {
        if (request.Url == null) return;

        var linkPreview = await _linkPreviewGenerator.GetLinkPreview(request.Url);

        if (linkPreview == null) return;

        await Task.WhenAll(
            _messageRepository.SetLinkPreviewAsync(request.MessageId, linkPreview.Id),
            SendNotification(request.RecipientId, request.SenderId, request.MessageId, linkPreview),
            SendNotification(request.SenderId, request.RecipientId, request.MessageId, linkPreview));
    }

    private Task SendNotification(int recipientId, int senderId, int messageId, Domain.LinkPreview linkPreview)
    {
        return _producer.ProduceAsync(
            _kafkaSettings.LinkPreviewNotification!.Topic,
            Guid.NewGuid().ToString(),
            new Notification<LinkPreviewDto>
            {
                RecipientId = recipientId,
                Message = new LinkPreviewDto
                {
                    AccountId = senderId,
                    MessageId = messageId,
                    Title = linkPreview.Title,
                    ImageUrl = linkPreview.ImageUrl,
                    Url = linkPreview.Url
                }
            });
    }
}

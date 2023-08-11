using KafkaFlow;
using KafkaFlow.Producers;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.LinkPreview.Abstractions;
using LetsTalk.Server.LinkPreview.Models;
using LetsTalk.Server.Notifications.Models;
using LetsTalk.Server.Persistence.Abstractions;
using Microsoft.Extensions.Options;

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
        if (request.Url == null)
        {
            return;
        }

        var linkPreview = await _linkPreviewGenerator.GetLinkPreviewAsync(request.Url);

        if (linkPreview == null)
        {
            return;
        }

        var linkPreviewDto = new LinkPreviewDto
        {
            MessageId = request.MessageId,
            Title = linkPreview.Title,
            ImageUrl = linkPreview.ImageUrl,
            Url = linkPreview.Url
        };

        await Task.WhenAll(
            _messageRepository.SetLinkPreviewAsync(request.MessageId, linkPreview.Id),
            _producer.ProduceAsync(
                _kafkaSettings.LinkPreviewNotification!.Topic,
                Guid.NewGuid().ToString(),
                new Notification<LinkPreviewDto>[]
                {
                    new Notification<LinkPreviewDto>
                    {
                        RecipientId = request.RecipientId,
                        Message = linkPreviewDto with
                        {
                            AccountId = request.SenderId
                        }
                    },
                    new Notification<LinkPreviewDto>
                    {
                        RecipientId = request.SenderId,
                        Message = linkPreviewDto with
                        {
                            AccountId = request.RecipientId
                        }
                    }
                }));
    }
}

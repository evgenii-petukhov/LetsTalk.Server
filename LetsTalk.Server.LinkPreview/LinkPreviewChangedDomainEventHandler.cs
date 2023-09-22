using KafkaFlow;
using KafkaFlow.Producers;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Domain.Events;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Notifications.Models;
using MediatR;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.LinkPreview;

public class LinkPreviewChangedDomainEventHandler : INotificationHandler<MessageDomainEvent<Domain.LinkPreview>>
{
    private readonly IMessageProducer _producer;
    private readonly KafkaSettings _kafkaSettings;

    public LinkPreviewChangedDomainEventHandler(
        IOptions<KafkaSettings> kafkaSettings,
        IProducerAccessor producerAccessor)
    {
        _kafkaSettings = kafkaSettings.Value;
        _producer = producerAccessor.GetProducer(_kafkaSettings.LinkPreviewNotification!.Producer);
    }

    public Task Handle(MessageDomainEvent<Domain.LinkPreview> notification, CancellationToken cancellationToken)
    {
        var linkPreviewDto = new LinkPreviewDto
        {
            MessageId = notification.Message!.Id,
            Title = notification.Payload!.Title,
            ImageUrl = notification.Payload.ImageUrl,
            Url = notification.Payload.Url
        };

        return _producer.ProduceAsync(
            _kafkaSettings.LinkPreviewNotification!.Topic,
            Guid.NewGuid().ToString(),
            new Notification<LinkPreviewDto>[]
            {
                new Notification<LinkPreviewDto>
                {
                    RecipientId = notification.Message.RecipientId,
                    Message = linkPreviewDto with
                    {
                        AccountId = notification.Message.SenderId
                    }
                },
                new Notification<LinkPreviewDto>
                {
                    RecipientId = notification.Message.SenderId,
                    Message = linkPreviewDto with
                    {
                        AccountId = notification.Message.RecipientId
                    }
                }
            });
    }
}

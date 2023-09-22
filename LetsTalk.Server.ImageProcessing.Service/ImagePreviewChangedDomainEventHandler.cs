using KafkaFlow;
using KafkaFlow.Producers;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Domain.Events;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Notifications.Models;
using MediatR;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.ImageProcessing.Service;

public class ImagePreviewChangedDomainEventHandler : INotificationHandler<MessageDomainEvent<Image>>
{
    private readonly IMessageProducer _producer;
    private readonly KafkaSettings _kafkaSettings;

    public ImagePreviewChangedDomainEventHandler(
        IOptions<KafkaSettings> kafkaSettings,
        IProducerAccessor producerAccessor)
    {
        _kafkaSettings = kafkaSettings.Value;
        _producer = producerAccessor.GetProducer(_kafkaSettings.ImagePreviewNotification!.Producer);
    }

    public Task Handle(MessageDomainEvent<Image> notification, CancellationToken cancellationToken)
    {
        var imagePreviewDto = new ImagePreviewDto
        {
            MessageId = notification.Message!.Id,
            Id = notification.Payload!.Id,
        };

        return _producer.ProduceAsync(
            _kafkaSettings.ImagePreviewNotification!.Topic,
            Guid.NewGuid().ToString(),
            new Notification<ImagePreviewDto>[]
            {
                new Notification<ImagePreviewDto>
                {
                    RecipientId = notification.Message.RecipientId,
                    Message = imagePreviewDto with
                    {
                        AccountId = notification.Message.SenderId
                    }
                },
                new Notification<ImagePreviewDto>
                {
                    RecipientId = notification.Message.SenderId,
                    Message = imagePreviewDto with
                    {
                        AccountId = notification.Message.RecipientId
                    }
                }
            });
    }
}

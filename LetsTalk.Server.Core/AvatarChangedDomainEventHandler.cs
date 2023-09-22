using KafkaFlow;
using KafkaFlow.Producers;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Domain.Events;
using LetsTalk.Server.Kafka.Models;
using MediatR;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.LinkPreview;

public class AvatarChangedDomainEventHandler : INotificationHandler<AvatarChangedDomainEvent>
{
    private readonly IMessageProducer _producer;
    private readonly KafkaSettings _kafkaSettings;

    public AvatarChangedDomainEventHandler(
        IOptions<KafkaSettings> kafkaSettings,
        IProducerAccessor producerAccessor)
    {
        _kafkaSettings = kafkaSettings.Value;
        _producer = producerAccessor.GetProducer(_kafkaSettings.MessageNotification!.Producer);
    }

    public Task Handle(AvatarChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        return _producer.ProduceAsync(
            _kafkaSettings.RemoveImageRequest!.Topic,
            Guid.NewGuid().ToString(),
            new RemoveImageRequest
            {
                ImageId = notification.PreviousImageId
            });
    }
}

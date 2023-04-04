using KafkaFlow;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.Kafka.Models;

namespace LetsTalk.Server.Notifications.Handlers;

public class LinkPreviewNotificationHandler : IMessageHandler<LinkPreviewNotification>
{
    public Task Handle(IMessageContext context, LinkPreviewNotification message)
    {
        throw new NotImplementedException();
    }
}

using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Notifications.Abstractions;
using MassTransit;

namespace LetsTalk.Server.Notifications.Handlers;

public class NotificationConsumer(
    INotificationService notificationService) : IConsumer<Notification>
{
    private readonly INotificationService _notificationService = notificationService;

    public async Task Consume(ConsumeContext<Notification> context)
    {
        if (context.Message.Message != null)
        {
            await SendNotificationAsync(context.Message.RecipientId!, context.Message.Message);
        }

        if (context.Message.LinkPreview != null)
        {
            await SendNotificationAsync(context.Message.RecipientId!, context.Message.LinkPreview);
        }

        if (context.Message.ImagePreview != null)
        {
            await SendNotificationAsync(context.Message.RecipientId!, context.Message.ImagePreview);
        }

        if (context.Message.Connection != null)
        {
            if (!string.IsNullOrWhiteSpace(context.Message.Connection.Offer))
            {
                await SendNotificationAsync(context.Message.RecipientId!, context.Message.Connection, "RtcSessionOffer");
            }

            if (!string.IsNullOrWhiteSpace(context.Message.Connection.Answer))
            {
                await SendNotificationAsync(context.Message.RecipientId!, context.Message.Connection, "RtcSessionAnswer");
            }
        }
    }

    private Task SendNotificationAsync<T>(string id, T payload)
    {
        return SendNotificationAsync(id!, payload, payload!.GetType().Name);
    }

    private Task SendNotificationAsync<T>(string id, T payload, string typeName)
    {
        return _notificationService.SendNotificationAsync(id!, payload, typeName);
    }
}

using LetsTalk.Server.Models.Kafka;
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

        if (context.Message.IncomingCall != null)
        {
            await SendNotificationAsync(context.Message.RecipientId!, context.Message.IncomingCall, nameof(context.Message.IncomingCall));
        }

        if (context.Message.EstablishConnection != null)
        {
            await SendNotificationAsync(context.Message.RecipientId!, context.Message.EstablishConnection, nameof(context.Message.EstablishConnection));
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

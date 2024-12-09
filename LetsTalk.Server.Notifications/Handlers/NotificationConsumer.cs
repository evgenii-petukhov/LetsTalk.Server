using LetsTalk.Server.Notifications.Abstractions;
using LetsTalk.Server.Notifications.Models;
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
    }

    private Task SendNotificationAsync<T>(string id, T payload)
    {
        return _notificationService.SendNotificationAsync(id!, payload, payload!.GetType().Name);
    }
}

using LetsTalk.Server.Notifications.Abstractions;
using LetsTalk.Server.Notifications.Models;
using MassTransit;

namespace LetsTalk.Server.Notifications.Handlers;

public class NotificationConsumer<T>(
    INotificationService notificationService) : IConsumer<Notification<T>>
    where T : class
{
    private readonly INotificationService _notificationService = notificationService;

    public Task Consume(ConsumeContext<Notification<T>> context)
    {
        return _notificationService.SendNotificationAsync(context.Message.RecipientId!, context.Message.Message!, typeof(T).Name);
    }
}

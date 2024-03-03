using KafkaFlow;
using LetsTalk.Server.Notifications.Abstractions;
using LetsTalk.Server.Notifications.Models;

namespace LetsTalk.Server.Notifications.Handlers;

public class NotificationHandler<T>(
    INotificationService notificationService) : IMessageHandler<Notification<T>[]>
    where T : class
{
    private readonly INotificationService _notificationService = notificationService;

    public Task Handle(IMessageContext context, Notification<T>[] notifications)
    {
        return Task.WhenAll(notifications.Select(notification => _notificationService.SendNotificationAsync(notification.RecipientId!, notification.Message!, typeof(T).Name)));
    }
}

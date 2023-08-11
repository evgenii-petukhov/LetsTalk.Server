using KafkaFlow;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.Notifications.Abstractions;
using LetsTalk.Server.Notifications.Models;

namespace LetsTalk.Server.Notifications.Handlers;

public class NotificationHandler<T> : IMessageHandler<Notification<T>[]>
    where T : class
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationHandler<T>> _logger;

    public NotificationHandler(
        INotificationService notificationService,
        ILogger<NotificationHandler<T>> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public Task Handle(IMessageContext context, Notification<T>[] notifications)
    {
        _logger.LogInformation("{@notifications}", notifications);

        return Task.WhenAll(notifications.Select(notification => _notificationService.SendNotificationAsync(notification.RecipientId, notification.Message!, typeof(T).Name)));
    }
}

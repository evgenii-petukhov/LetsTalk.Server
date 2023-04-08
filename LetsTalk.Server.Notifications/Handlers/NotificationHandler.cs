using KafkaFlow.TypedHandler;
using KafkaFlow;
using LetsTalk.Server.Notifications.Abstractions;
using LetsTalk.Server.Notifications.Models;

namespace LetsTalk.Server.Notifications.Handlers;

public abstract class NotificationHandler<T> : IMessageHandler<Notification<T>> where T:class
{
    protected readonly INotificationService _notificationService;

    protected NotificationHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public abstract Task Handle(IMessageContext context, Notification<T> notification);
}

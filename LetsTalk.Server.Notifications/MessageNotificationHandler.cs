using KafkaFlow.TypedHandler;
using KafkaFlow;
using LetsTalk.Server.Notifications.Models;
using LetsTalk.Server.Notifications.Abstractions;

namespace LetsTalk.Server.Notifications;

public class MessageNotificationHandler : IMessageHandler<MessageNotification>
{
    private readonly INotificationService _notificationService;

    public MessageNotificationHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(IMessageContext context, MessageNotification notification)
    {
        await _notificationService.SendMessageNotification(notification.RecipientId, notification.Message!);
    }
}

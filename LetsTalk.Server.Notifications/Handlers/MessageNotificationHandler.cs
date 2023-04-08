using KafkaFlow;
using LetsTalk.Server.Notifications.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Notifications.Models;

namespace LetsTalk.Server.Notifications.Handlers;

public class MessageNotificationHandler : NotificationHandler<MessageDto>
{
    public MessageNotificationHandler(INotificationService notificationService) : base(notificationService)
    {
    }

    public override Task Handle(IMessageContext context, Notification<MessageDto> notification)
    {
        return _notificationService.SendMessageNotification(notification.RecipientId, notification.Message!);
    }
}

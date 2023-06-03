using KafkaFlow;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Notifications.Abstractions;
using LetsTalk.Server.Notifications.Models;

namespace LetsTalk.Server.Notifications.Handlers;

public class LinkPreviewNotificationHandler : NotificationHandler<LinkPreviewDto>
{
    public LinkPreviewNotificationHandler(INotificationService notificationService) : base(notificationService)
    {
    }

    public override Task Handle(IMessageContext context, Notification<LinkPreviewDto> notification)
    {
        return _notificationService.SendLinkPreviewNotificationAsync(notification.RecipientId, notification.Message!);
    }
}

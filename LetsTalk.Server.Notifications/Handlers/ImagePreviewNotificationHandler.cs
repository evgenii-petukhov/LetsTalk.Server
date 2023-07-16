using KafkaFlow;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Notifications.Abstractions;
using LetsTalk.Server.Notifications.Models;

namespace LetsTalk.Server.Notifications.Handlers;

public class ImagePreviewNotificationHandler : NotificationHandler<ImagePreviewDto>
{
    public ImagePreviewNotificationHandler(INotificationService notificationService) : base(notificationService)
    {
    }

    public override Task Handle(IMessageContext context, Notification<ImagePreviewDto> notification)
    {
        return _notificationService.SendImagePreviewNotificationAsync(notification.RecipientId, notification.Message!);
    }
}

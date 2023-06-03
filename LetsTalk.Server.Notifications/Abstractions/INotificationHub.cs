using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Notifications.Abstractions;

public interface INotificationHub
{
    Task SendMessageNotificationAsync(MessageDto notification);

    Task SendLinkPreviewNotificationAsync(LinkPreviewDto notification);
}
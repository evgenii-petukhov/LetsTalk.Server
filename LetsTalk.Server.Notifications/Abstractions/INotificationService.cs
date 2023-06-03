using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Notifications.Abstractions;

public interface INotificationService
{
    Task SendMessageNotificationAsync(int accountId, MessageDto notification);

    Task SendLinkPreviewNotificationAsync(int accountId, LinkPreviewDto notification);
}
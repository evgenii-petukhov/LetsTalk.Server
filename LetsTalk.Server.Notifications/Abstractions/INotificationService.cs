using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Notifications.Abstractions;

public interface INotificationService
{
    Task SendMessageNotification(int accountId, MessageDto message);
}
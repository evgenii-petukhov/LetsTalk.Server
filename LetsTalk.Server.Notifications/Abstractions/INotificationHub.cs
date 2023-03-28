using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Notifications.Abstractions;

public interface INotificationHub
{
    Task SendMessageNotification(MessageDto message);
}
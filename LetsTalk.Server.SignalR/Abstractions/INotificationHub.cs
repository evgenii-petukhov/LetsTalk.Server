using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.SignalR.Abstractions;

public interface INotificationHub
{
    Task SendMessageNotification(MessageDto message);
}

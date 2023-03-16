using LetsTalk.Server.Models.Message;

namespace LetsTalk.Server.SignalR.Abstractions;

public interface INotificationHub
{
    Task SendMessageNotification(MessageDto message);
}

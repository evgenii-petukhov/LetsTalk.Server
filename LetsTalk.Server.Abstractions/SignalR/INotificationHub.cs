using LetsTalk.Server.Models.Message;

namespace LetsTalk.Server.Abstractions.SignalR;

public interface INotificationHub
{
    Task SendMessageNotification(MessageDto message);
}

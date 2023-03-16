using LetsTalk.Server.Models.Message;

namespace LetsTalk.Server.SignalR.Abstractions;

public interface INotificationService
{
    Task SendMessageNotification(int accountId, MessageDto message);
}

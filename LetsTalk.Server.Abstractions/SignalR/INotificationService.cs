using LetsTalk.Server.Models.Message;

namespace LetsTalk.Server.Abstractions.SignalR;

public interface INotificationService
{
    Task SendMessageNotification(int accountId, MessageDto message);
}

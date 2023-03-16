using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.SignalR.Abstractions;

public interface INotificationService
{
    Task SendMessageNotification(int accountId, MessageDto message);
}

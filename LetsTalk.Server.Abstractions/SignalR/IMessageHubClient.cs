using LetsTalk.Server.Models.Message;

namespace LetsTalk.Server.Abstractions.SignalR;

public interface IMessageHubClient
{
    Task SendMessageNotification(MessageDto message);
}

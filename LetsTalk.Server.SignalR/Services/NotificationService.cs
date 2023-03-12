using LetsTalk.Server.Abstractions.SignalR;
using LetsTalk.Server.Models.Message;
using LetsTalk.Server.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace LetsTalk.Server.SignalR.Services;

public class NotificationService : INotificationService
{
    private readonly IConnectionManager _connectionManager;
    private readonly IHubContext<NotificationHub, INotificationHub> _messageHub;

    public NotificationService(
        IConnectionManager connectionManager,
        IHubContext<NotificationHub, INotificationHub> messageHub)
    {
        _connectionManager = connectionManager;
        _messageHub = messageHub;
    }

    public async Task SendMessageNotification(int accountId, MessageDto message)
    {
        var connectionId = _connectionManager.GetConnectionId(accountId);
        if (connectionId != null)
        {
            await _messageHub.Clients.Client(connectionId).SendMessageNotification(message);
        }
    }
}

using LetsTalk.Server.Notifications.Abstractions;
using LetsTalk.Server.Notifications.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace LetsTalk.Server.Notifications.Services;

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

    public Task SendNotificationAsync<T>(string accountId, T notification, string typeName)
    {
        var notifications = _connectionManager.GetConnectionIds(accountId)?
            .Select(connectionId => _messageHub.Clients.Client(connectionId).SendNotificationAsync(notification, typeName));
        return notifications == null ? Task.CompletedTask : Task.WhenAll(notifications);
    }
}

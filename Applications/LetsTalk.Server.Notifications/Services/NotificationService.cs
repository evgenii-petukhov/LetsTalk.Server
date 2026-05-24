using LetsTalk.Server.Notifications.Abstractions;
using LetsTalk.Server.Notifications.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace LetsTalk.Server.Notifications.Services;

public class NotificationService(
    IConnectionManager connectionManager,
    IHubContext<NotificationHub, INotificationHub> messageHub) : INotificationService
{
    private readonly IConnectionManager _connectionManager = connectionManager;
    private readonly IHubContext<NotificationHub, INotificationHub> _messageHub = messageHub;

    public Task SendNotificationAsync<T>(string accountId, T notification, string typeName)
    {
        var notifications = _connectionManager.GetConnectionIds(accountId)?
            .Select(connectionId => _messageHub.Clients.Client(connectionId).SendNotificationAsync(notification, typeName));
        return notifications == null ? Task.CompletedTask : Task.WhenAll(notifications);
    }
}

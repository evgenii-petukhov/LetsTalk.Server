using LetsTalk.Server.Dto.Models;
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

    public Task SendLinkPreviewNotificationAsync(int accountId, LinkPreviewDto notification)
    {
        var notifications = _connectionManager.GetConnectionIds(accountId)?
            .Select(connectionId => _messageHub.Clients.Client(connectionId).SendLinkPreviewNotificationAsync(notification));
        return notifications == null ? Task.CompletedTask : Task.WhenAll(notifications);
    }

    public Task SendMessageNotificationAsync(int accountId, MessageDto notification)
    {
        var notifications = _connectionManager.GetConnectionIds(accountId)?
            .Select(connectionId => _messageHub.Clients.Client(connectionId).SendMessageNotificationAsync(notification));
        return notifications == null ? Task.CompletedTask : Task.WhenAll(notifications);
    }

    public Task SendImagePreviewNotificationAsync(int accountId, ImagePreviewDto notification)
    {
        var notifications = _connectionManager.GetConnectionIds(accountId)?
            .Select(connectionId => _messageHub.Clients.Client(connectionId).SendImagePreviewNotificationAsync(notification));
        return notifications == null ? Task.CompletedTask : Task.WhenAll(notifications);
    }
}

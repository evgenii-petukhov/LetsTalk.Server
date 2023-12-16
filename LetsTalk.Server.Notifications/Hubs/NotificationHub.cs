using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Notifications.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace LetsTalk.Server.Notifications.Hubs;

public class NotificationHub : Hub<INotificationHub>
{
    private readonly IConnectionManager _connectionManager;
    private readonly IAuthenticationClient _authenticationClient;

    public NotificationHub(
        IConnectionManager connectionManager,
        IAuthenticationClient authenticationClient)
    {
        _connectionManager = connectionManager;
        _authenticationClient = authenticationClient;
    }

    public async Task AuthorizeAsync(string token)
    {
        var accountId = await _authenticationClient.ValidateJwtTokenAsync(token);
        if (accountId != null)
        {
            _connectionManager.AddConnectionId(accountId, Context.ConnectionId);
        }
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _connectionManager.RemoveConnectionId(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
using LetsTalk.Server.Abstractions.Authentication;
using LetsTalk.Server.Abstractions.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace LetsTalk.Server.SignalR.Hubs;

public class NotificationHub : Hub<INotificationHub>
{
    private readonly IJwtService _jwtService;
    private readonly IConnectionManager _connectionManager;

    public NotificationHub(
        IJwtService jwtService,
        IConnectionManager connectionManager)
    {
        _jwtService = jwtService;
        _connectionManager = connectionManager;
    }

    public void Authorize(string token)
    {
        var accountId = _jwtService.ValidateJwtToken(token);
        if (accountId != null)
        {
            _connectionManager.SetConnectionId(accountId.Value, Context.ConnectionId);
        }
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _connectionManager.RemoveConnectionId(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
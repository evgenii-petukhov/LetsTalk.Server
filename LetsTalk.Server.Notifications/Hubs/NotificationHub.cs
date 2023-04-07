using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Notifications.Abstractions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Notifications.Hubs;

public class NotificationHub : Hub<INotificationHub>
{
    private readonly IConnectionManager _connectionManager;
    private readonly IAuthenticationClient _authenticationClient;
    private readonly AuthenticationSettings _authenticationSettings;

    public NotificationHub(
        IConnectionManager connectionManager,
        IAuthenticationClient authenticationClient,
        IOptions<AuthenticationSettings> options)
    {
        _connectionManager = connectionManager;
        _authenticationClient = authenticationClient;
        _authenticationSettings = options.Value;
    }

    public async Task Authorize(string token)
    {
        var accountId = await _authenticationClient.ValidateJwtToken(_authenticationSettings.Url!, token);
        if (accountId != null)
        {
            _connectionManager.AddConnectionId(accountId.Value, Context.ConnectionId);
        }
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _connectionManager.RemoveConnectionId(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
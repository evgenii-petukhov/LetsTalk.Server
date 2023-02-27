using LetsTalk.Server.Abstractions.Authentication;
using LetsTalk.Server.Abstractions.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace LetsTalk.Server.API.SignalR;

public class MessageHub : Hub<IMessageHubClient>
{
    private readonly IJwtService _jwtService;
    private readonly IMessageHubConnectionManager _connectionManager;

    public MessageHub(
        IJwtService jwtService,
        IMessageHubConnectionManager connectionManager)
    {
        _jwtService = jwtService;
        _connectionManager = connectionManager;
    }

    public async Task SendOffersToUser(List<string> message)
    {
        await Clients.All.SendOffersToUser(message);
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
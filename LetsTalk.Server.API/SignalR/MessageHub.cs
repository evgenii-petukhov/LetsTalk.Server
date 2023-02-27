using LetsTalk.Server.Abstractions.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace LetsTalk.Server.API.SignalR;

public class MessageHub : Hub<IMessageHubClient>
{
    public async Task SendOffersToUser(List<string> message)
    {
        await Clients.All.SendOffersToUser(message);
    }
}
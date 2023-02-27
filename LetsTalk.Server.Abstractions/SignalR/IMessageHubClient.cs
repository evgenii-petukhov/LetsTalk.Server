namespace LetsTalk.Server.Abstractions.SignalR;

public interface IMessageHubClient
{
    Task SendOffersToUser(List<string> message);
}

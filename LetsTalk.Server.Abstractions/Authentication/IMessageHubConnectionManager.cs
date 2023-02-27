namespace LetsTalk.Server.Abstractions.Authentication;

public interface IMessageHubConnectionManager
{
    string GetConnectionId(int accountId);

    void SetConnectionId(int accountId, string connectionId);

    void RemoveConnectionId(string connectionId);
}

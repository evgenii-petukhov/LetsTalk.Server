namespace LetsTalk.Server.Notifications.Abstractions;

public interface IConnectionManager
{
    string GetConnectionId(int accountId);

    void SetConnectionId(int accountId, string connectionId);

    void RemoveConnectionId(string connectionId);
}

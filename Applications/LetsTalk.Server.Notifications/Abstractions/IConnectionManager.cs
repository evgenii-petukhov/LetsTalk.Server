namespace LetsTalk.Server.Notifications.Abstractions;

public interface IConnectionManager
{
    HashSet<string> GetConnectionIds(string accountId);

    void AddConnectionId(string accountId, string connectionId);

    void RemoveConnectionId(string connectionId);
}

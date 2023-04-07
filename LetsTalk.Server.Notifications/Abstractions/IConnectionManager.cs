namespace LetsTalk.Server.Notifications.Abstractions;

public interface IConnectionManager
{
    HashSet<string> GetConnectionIds(int accountId);

    void AddConnectionId(int accountId, string connectionId);

    void RemoveConnectionId(string connectionId);
}

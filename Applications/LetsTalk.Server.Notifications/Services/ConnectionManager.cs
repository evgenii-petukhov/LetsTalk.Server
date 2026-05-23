using LetsTalk.Server.Notifications.Abstractions;

namespace LetsTalk.Server.Notifications.Services;

public class ConnectionManager : IConnectionManager
{
    private readonly Lock _lock = new();

    public Dictionary<string, HashSet<string>> ConnectionIdAccountIdBy { get; } = new(StringComparer.Ordinal);

    public HashSet<string> GetConnectionIds(string accountId)
    {
        return ConnectionIdAccountIdBy.GetValueOrDefault(accountId)!;
    }

    public void RemoveConnectionId(string connectionId)
    {
        lock (_lock)
        {
            var isFound = ConnectionIdAccountIdBy
                .Any(x => x.Value.Contains(connectionId));

            if (isFound)
            {
                var mapping = ConnectionIdAccountIdBy
                    .First(x => x.Value.Contains(connectionId));

                mapping.Value.RemoveWhere(x => string.Equals(x, connectionId, StringComparison.Ordinal));

                if (mapping.Value.Count == 0)
                {
                    ConnectionIdAccountIdBy.Remove(mapping.Key);
                }
            }
        }
    }

    public void AddConnectionId(string accountId, string connectionId)
    {
        lock (_lock)
        {
            if (ConnectionIdAccountIdBy.TryGetValue(accountId, out var value))
            {
                value.Add(connectionId);
            }
            else
            {
                ConnectionIdAccountIdBy.Add(
                    accountId,
                    new HashSet<string>(StringComparer.Ordinal)
                    {
                        connectionId
                    });
            }
        }
    }
}
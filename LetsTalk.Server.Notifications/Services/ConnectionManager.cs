using LetsTalk.Server.Notifications.Abstractions;

namespace LetsTalk.Server.Notifications.Services;

public class ConnectionManager : IConnectionManager
{
    private readonly object _lock = new();

    public readonly Dictionary<int, HashSet<string>> _connectionIdAccountIdBy = new();

    public HashSet<string> GetConnectionIds(int accountId)
    {
        return _connectionIdAccountIdBy.GetValueOrDefault(accountId)!;
    }

    public void RemoveConnectionId(string connectionId)
    {
        lock (_lock)
        {
            var isFound = _connectionIdAccountIdBy
                .Any(x => x.Value.Contains(connectionId, StringComparer.OrdinalIgnoreCase));

            if (isFound)
            {
                var mapping = _connectionIdAccountIdBy
                    .First(x => x.Value.Contains(connectionId, StringComparer.OrdinalIgnoreCase));

                var newValue = mapping.Value
                    .Where(x => !string.Equals(x, connectionId, StringComparison.OrdinalIgnoreCase))
                    .ToHashSet();

                if (newValue.Any())
                {
                    _connectionIdAccountIdBy[mapping.Key] = newValue;
                }
                else
                {
                    _connectionIdAccountIdBy.Remove(mapping.Key);
                }
            }
        }
    }

    public void AddConnectionId(int accountId, string connectionId)
    {
        lock (_lock)
        {
            if (_connectionIdAccountIdBy.TryGetValue(accountId, out var value))
            {
                value.Add(connectionId);
            }
            else
            {
                _connectionIdAccountIdBy.Add(accountId, new HashSet<string> { connectionId });
            }
        }
    }
}
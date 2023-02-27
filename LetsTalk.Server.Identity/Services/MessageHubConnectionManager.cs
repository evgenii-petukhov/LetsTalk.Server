using LetsTalk.Server.Abstractions.Authentication;
using System.Collections.Concurrent;

namespace LetsTalk.Server.Identity.Services
{
    public class MessageHubConnectionManager: IMessageHubConnectionManager
    {
        public readonly ConcurrentDictionary<int, string> _connectionIdAccountIdBy = new();

        public string GetConnectionId(int accountId)
        {
            return _connectionIdAccountIdBy.GetValueOrDefault(accountId);
        }

        public void RemoveConnectionId(string connectionId)
        {
            _connectionIdAccountIdBy
                .Where(x => string.Equals(x.Value, connectionId, StringComparison.OrdinalIgnoreCase))
                .ToList()
                .ForEach(x => _connectionIdAccountIdBy.TryRemove(x));
            
        }

        public void SetConnectionId(int accountId, string connectionId)
        {
            _connectionIdAccountIdBy[accountId] = connectionId;
        }
    }
}

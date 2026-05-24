using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions.Models;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

public interface IChatRepository : IGenericRepository<Chat>
{
    Task<List<Chat>> GetChatsByAccountIdAsync(int accountId, CancellationToken cancellationToken = default);

    Task<Dictionary<int, ChatMetric>> GetChatMetricsAsync(int accountId, CancellationToken cancellationToken = default);

    Task<bool> IsChatIdValidAsync(int id, CancellationToken cancellationToken = default);

    Task<Chat> GetIndividualChatByAccountIdsAsync(IEnumerable<int> accountIds, CancellationToken cancellationToken = default);

    Task<List<int>> GetAccountIdsInIndividualChatsAsync(int accountId, CancellationToken cancellationToken = default);
}

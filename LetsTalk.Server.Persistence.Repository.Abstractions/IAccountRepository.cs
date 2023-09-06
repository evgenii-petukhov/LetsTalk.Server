using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;
using System.Linq.Expressions;

namespace LetsTalk.Server.Persistence.Repository.Abstractions;

public interface IAccountRepository : IGenericRepository<Account>
{
    Task<int> CreateOrUpdateAsync(string externalId, AccountTypes accountType, string? firstName, string? lastName,
        string? email, string? photoUrl, CancellationToken cancellationToken = default);

    Task<T?> GetByIdOrDefaultAsync<T>(int id, Expression<Func<Account, T>> selector, bool includeFile = false, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AccountWithUnreadCount>> GetOthersAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default);

    Task UpdateAsync(int accountId, string? firstName, string? lastName, string? email, CancellationToken cancellationToken = default);

    Task UpdateAsync(int accountId, string? firstName, string? lastName, string? email, int? imageId, CancellationToken cancellationToken = default);
}

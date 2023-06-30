using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Persistence.Abstractions;

public interface IAccountRepository : IGenericRepository<Account>
{
    IQueryable<Account> GetByExternalId(string externalId, AccountTypes accountTypes);

    IQueryable<Account> GetById(int id, bool includeImage = false, bool includeFile = false);

    Task<IReadOnlyList<AccountWithUnreadCount>> GetOthersAsync(int id, CancellationToken cancellationToken = default);

    Task SetImageIdAsync(int accountId, int imageId, CancellationToken cancellationToken = default);

    Task UpdateAsync(int accountId, string? firstName, string? lastName, string? email, CancellationToken cancellationToken = default);

    Task UpdateAsync(int accountId, string? firstName, string? lastName, string? email, string? photoUrl, CancellationToken cancellationToken = default);
}

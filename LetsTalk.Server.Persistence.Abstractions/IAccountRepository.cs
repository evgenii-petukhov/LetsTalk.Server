using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Persistence.Abstractions;

public interface IAccountRepository : IGenericRepository<Account>
{
    IQueryable<Account> GetByExternalId(string externalId, AccountTypes accountTypes);

    Task<Account?> GetByIdIncludingFilesAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AccountWithUnreadCount>> GetOtherAsync(int id, CancellationToken cancellationToken = default);

    Task<Account?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task SetImageIdAsync(int accountId, int imageId, CancellationToken cancellationToken = default);

    Task UpdateAsync(int accountId, string? firstName, string? lastName, string? email, CancellationToken cancellationToken = default);

    Task UpdateAsync(int accountId, string? firstName, string? lastName, string? email, string? photoUrl, CancellationToken cancellationToken = default);

    Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default);
}

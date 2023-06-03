using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Models;

namespace LetsTalk.Server.Persistence.Abstractions;

public interface IAccountRepository : IGenericRepository<Account>
{
    Task<Account?> GetByExternalIdAsync(string externalId, AccountTypes accountTypes, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AccountWithUnreadCount>> GetOtherAsync(int id, CancellationToken cancellationToken = default);

    Task<Account?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task UpdateAsync(int accountId, string? firstName, string? lastName, CancellationToken cancellationToken = default);

    Task UpdateAsync(int accountId, string? firstName, string? lastName, string? email, CancellationToken cancellationToken = default);

    Task UpdateAsync(int accountId, string? firstName, string? lastName, string? email, string? photoUrl, CancellationToken cancellationToken = default);

    Task UpdateAsync(int accountId, string? firstName, string? lastName, string? email, string? photoUrl, int? imageId, CancellationToken cancellationToken = default);

    Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default);
}

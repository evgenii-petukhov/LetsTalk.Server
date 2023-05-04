using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Models;

namespace LetsTalk.Server.Persistence.Abstractions;

public interface IAccountRepository : IGenericRepository<Account>
{
    Task<Account?> GetByExternalIdAsync(string externalId, AccountTypes accountTypes);

    Task<IReadOnlyList<AccountWithUnreadCount>> GetOtherAsync(int id);

    Task<Account?> GetByIdAsync(int id);

    Task UpdateAsync(int accountId, string? firstName, string? lastName, string? photoUrl, string? email);

    Task UpdateAsync(int accountId, string? firstName, string? lastName, string? email);

    Task<bool> IsAccountIdValidAsync(int id);
}

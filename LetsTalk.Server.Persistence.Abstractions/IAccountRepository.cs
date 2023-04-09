using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.Abstractions;

public interface IAccountRepository : IGenericRepository<Account>
{
    Task<Account?> GetByExternalIdAsync(string externalId);

    Task<IReadOnlyList<AccountWithUnreadCount>> GetOtherAsync(int id);

    Task<Account?> GetByIdAsync(int id);

    Task UpdateAsync(int accountId, string? firstName, string? lastName, string? photoUrl);
}

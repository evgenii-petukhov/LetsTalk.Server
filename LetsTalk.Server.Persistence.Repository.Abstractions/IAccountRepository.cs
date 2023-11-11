using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.Repository.Abstractions.Models;

namespace LetsTalk.Server.Persistence.Repository.Abstractions;

public interface IAccountRepository : IGenericRepository<Account>
{
    Task<Account> GetByExternalIdAsync(string externalId, AccountTypes accountType, CancellationToken cancellationToken = default);

    Task<Account> GetByExternalIdAsTrackingAsync(string externalId, AccountTypes accountType, CancellationToken cancellationToken = default);

    Task<List<AccountListItem>> GetContactsAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default);
}

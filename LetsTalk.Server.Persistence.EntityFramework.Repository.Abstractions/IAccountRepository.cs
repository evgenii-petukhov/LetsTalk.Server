using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

public interface IAccountRepository : IGenericRepository<Account>
{
    Task<Account> GetByExternalIdAsync(string externalId, AccountTypes accountType, CancellationToken cancellationToken = default);

    Task<Account> GetByExternalIdAsTrackingAsync(string externalId, AccountTypes accountType, CancellationToken cancellationToken = default);

    Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default);
}

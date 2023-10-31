using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.Repository.Abstractions.Models;
using System.Linq.Expressions;

namespace LetsTalk.Server.Persistence.Repository.Abstractions;

public interface IAccountRepository : IGenericRepository<Account>
{
    Task<Account> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync<T>(int id, Expression<Func<Account, T>> selector, CancellationToken cancellationToken = default);

    Task<Account> GetByExternalIdAsync(string externalId, AccountTypes accountType, CancellationToken cancellationToken = default);

    Task<Account> GetByExternalIdAsTrackingAsync(string externalId, AccountTypes accountType, CancellationToken cancellationToken = default);

    Task<List<AccountListItem>> GetOthersAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default);
}

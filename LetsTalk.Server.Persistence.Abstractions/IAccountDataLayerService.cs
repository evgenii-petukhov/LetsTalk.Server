using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;
using System.Linq.Expressions;

namespace LetsTalk.Server.Persistence.Abstractions;

public interface IAccountDataLayerService
{
    Task<int> CreateOrUpdateAsync(string externalId, AccountTypes accountType, string? firstName, string? lastName, string? email, string? photoUrl, CancellationToken cancellationToken);

    Task<T?> GetByIdOrDefaultAsync<T>(int id, Expression<Func<Account, T>> selector, bool includeFile = false, CancellationToken cancellationToken = default);

    Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default);
}

using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Persistence.Abstractions;

public interface IAccountDataLayerService
{
    Task<int> CreateOrUpdateAsync(string externalId, AccountTypes accountType, string? firstName, string? lastName, string? email, string? photoUrl, CancellationToken cancellationToken);

    Task<Account?> GetByIdAsync(int id, bool includeImage = false, bool includeFile = false, CancellationToken cancellationToken = default);

    Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default);
}

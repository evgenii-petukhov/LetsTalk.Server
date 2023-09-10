using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Core.Abstractions;

public interface IAccountDataLayerService
{
    Task<int> CreateOrUpdateAsync(string externalId, AccountTypes accountType, string? firstName, string? lastName,
        string? email, string? photoUrl, CancellationToken cancellationToken = default);
}

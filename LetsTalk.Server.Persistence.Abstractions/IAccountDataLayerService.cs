using LetsTalk.Server.Persistence.Models;

namespace LetsTalk.Server.Persistence.Abstractions;

public interface IAccountDataLayerService
{
    Task<int> CreateOrUpdateAsync(string externalId, AccountTypes accountType, string? firstName, string? lastName, string? photoUrl, string? email = null);

    Task UpdateAsync(int id, string? firstName, string? lastName, string? email, int? imageId);
}

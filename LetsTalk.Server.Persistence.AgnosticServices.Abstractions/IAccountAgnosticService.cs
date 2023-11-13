using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

public interface IAccountAgnosticService
{
    Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default);

    Task<AccountServiceModel> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> CreateOrUpdateAsync(
        string externalId,
        AccountTypes accountType,
        string firstName,
        string lastName,
        string email,
        string photoUrl,
        CancellationToken cancellationToken = default);

    Task<AccountServiceModel> UpdateProfileAsync(
        int accountId,
        string firstName,
        string lastName,
        string email,
        int? imageId,
        CancellationToken cancellationToken = default);
}

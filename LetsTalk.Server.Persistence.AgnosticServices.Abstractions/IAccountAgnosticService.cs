using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

public interface IAccountAgnosticService
{
    Task<ProfileServiceModel> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<List<AccountServiceModel>> GetAccountsAsync(CancellationToken cancellationToken = default);

    Task<string> CreateOrUpdateAsync(
        string externalId,
        AccountTypes accountType,
        string firstName,
        string lastName,
        string photoUrl,
        CancellationToken cancellationToken = default);

    Task<string> GetOrCreateAsync(
        AccountTypes accountType,
        string email,
        CancellationToken cancellationToken = default);

    Task<ProfileServiceModel> UpdateProfileAsync(
        string accountId,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default);

    Task<ProfileServiceModel> UpdateProfileAsync(
        string accountId,
        string firstName,
        string lastName,
        string imageId,
        int width,
        int height,
        ImageFormats imageFormat,
        CancellationToken cancellationToken = default);

    Task<bool> IsAccountIdValidAsync(string id, CancellationToken cancellationToken = default);
}

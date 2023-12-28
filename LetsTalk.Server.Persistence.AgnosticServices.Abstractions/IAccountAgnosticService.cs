using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

public interface IAccountAgnosticService
{
    Task<bool> IsAccountIdValidAsync(string id, CancellationToken cancellationToken = default);

    Task<AccountServiceModel> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<string> CreateOrUpdateAsync(
        string externalId,
        AccountTypes accountType,
        string firstName,
        string lastName,
        string email,
        string photoUrl,
        CancellationToken cancellationToken = default);

    Task<AccountServiceModel> UpdateProfileAsync(
        string accountId,
        string firstName,
        string lastName,
        string email,
        CancellationToken cancellationToken = default);

    Task<AccountServiceModel> UpdateProfileAsync(
        string accountId,
        string firstName,
        string lastName,
        string email,
        string imageId,
        int width,
        int height,
        ImageFormats imageFormat,
        bool updateImage,
        CancellationToken cancellationToken = default);

    Task<List<ContactServiceModel>> GetContactsAsync(string id, CancellationToken cancellationToken = default);
}

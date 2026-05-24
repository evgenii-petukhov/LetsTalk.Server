using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

public interface IProfileAgnosticService
{
    Task<ProfileServiceModel> GetByIdAsync(string id, CancellationToken cancellationToken = default);

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
        FileStorageTypes fileStorageType,
        CancellationToken cancellationToken = default);
}

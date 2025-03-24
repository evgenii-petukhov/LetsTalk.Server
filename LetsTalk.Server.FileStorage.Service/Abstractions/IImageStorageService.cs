using LetsTalk.Server.FileStorage.Service.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Service.Abstractions;

public interface IImageStorageService
{
    Task<FetchImageResponse?> GetImageAsync(string imageId, FileStorageTypes fileStorageType, CancellationToken cancellationToken = default);
}

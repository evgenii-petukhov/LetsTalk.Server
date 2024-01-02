using LetsTalk.Server.FileStorage.Utility.Abstractions.Models;

namespace LetsTalk.Server.FileStorage.Utility.Abstractions;

public interface IImageService
{
    Task<FetchImageResponse?> FetchImageAsync(string imageId, CancellationToken cancellationToken = default);
}

using LetsTalk.Server.FileStorage.AgnosticServices.Abstractions.Models;

namespace LetsTalk.Server.FileStorage.AgnosticServices.Abstractions;

public interface IImageService
{
    Task<FetchImageResponse?> FetchImageAsync(string imageId, CancellationToken cancellationToken = default);
}

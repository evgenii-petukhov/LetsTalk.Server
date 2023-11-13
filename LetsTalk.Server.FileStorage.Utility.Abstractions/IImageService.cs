using LetsTalk.Server.FileStorage.Utility.Abstractions.Models;

namespace LetsTalk.Server.FileStorage.Utility.Abstractions;

public interface IImageService
{
    Task<FetchImageResponse> FetchImageAsync(int imageId, CancellationToken cancellationToken = default);
}

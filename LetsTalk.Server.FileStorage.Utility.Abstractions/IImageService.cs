using LetsTalk.Server.FileStorage.Models;

namespace LetsTalk.Server.FileStorage.Utility.Abstractions;

public interface IImageService
{
    Task<FetchImageResponse> FetchImageAsync(int imageId, bool useDimensions = false, CancellationToken cancellationToken = default);
}

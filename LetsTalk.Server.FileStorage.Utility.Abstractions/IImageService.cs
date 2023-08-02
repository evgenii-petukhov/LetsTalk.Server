using LetsTalk.Server.FileStorage.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Utility.Abstractions;

public interface IImageService
{
    Task<int> SaveImageAsync(byte[] content, ImageRoles imageRole, ImageFormats imageFormat, int width, int height, CancellationToken cancellationToken = default);

    Task<FetchImageResponse> FetchImageAsync(int imageId, bool useDimensions = false, CancellationToken cancellationToken = default);
}

using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorageService.Abstractions;

public interface IImageService
{
    Task SaveImageAsync(byte[] content, ImageTypes imageType, int accountId, CancellationToken cancellationToken = default);

    Task<byte[]> FetchImageAsync(int imageId, CancellationToken cancellationToken = default);
}

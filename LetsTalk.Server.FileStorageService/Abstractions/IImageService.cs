using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorageService.Abstractions;

public interface IImageService
{
    Task<int> SaveImageAsync(byte[] content, ImageRoles imageRole, ImageFormats imageFormat, int accountId, CancellationToken cancellationToken = default);

    Task<byte[]> FetchImageAsync(int imageId, CancellationToken cancellationToken = default);
}

using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorageService.Abstractions;

public interface IFileService
{
    Task<byte[]> ReadFileAsync(string filename, FileTypes fileType, CancellationToken cancellationToken = default);

    Task<string?> SaveDataAsync(byte[] data, FileTypes fileType, CancellationToken cancellationToken = default);

    void DeleteFile(string filename, FileTypes fileType);
}

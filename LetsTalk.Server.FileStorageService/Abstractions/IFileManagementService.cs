using LetsTalk.Server.FileStorage.Models;

namespace LetsTalk.Server.FileStorageService.Abstractions;

public interface IFileManagementService
{
    Task<byte[]> GetFileContentAsync(string filename, FileStorageItemType fileType, CancellationToken cancellationToken = default);

    Task<FilePathInfo> SaveFileAsync(byte[] data, FileStorageItemType fileType, CancellationToken cancellationToken = default);

    void DeleteFile(string filename, FileStorageItemType fileType);
}

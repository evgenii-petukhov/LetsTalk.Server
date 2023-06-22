using LetsTalk.Server.FileStorageService.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorageService.Abstractions;

public interface IFileManagementService
{
    Task<byte[]> GetFileContentAsync(string filename, FileTypes fileType, CancellationToken cancellationToken = default);

    Task<FilePathInfo> SaveFileAsync(byte[] data, FileTypes fileType, CancellationToken cancellationToken = default);

    void DeleteFile(string filename, FileTypes fileType);
}

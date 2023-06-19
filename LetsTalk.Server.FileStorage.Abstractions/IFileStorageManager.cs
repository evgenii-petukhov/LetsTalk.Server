using LetsTalk.Server.FileStorage.Models;

namespace LetsTalk.Server.FileStorage.Abstractions;

public interface IFileStorageManager
{
    Task<byte[]> GetImageContentAsync(string filename, CancellationToken cancellationToken);

    Task<FilePathInfo> SaveImageAsync(byte[] data, CancellationToken cancellationToken);

    void DeleteImage(string filename);
}

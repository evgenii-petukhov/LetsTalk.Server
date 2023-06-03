using LetsTalk.Server.FileStorage.Models;
using LetsTalk.Server.Persistence.Models;

namespace LetsTalk.Server.FileStorage.Abstractions;

public interface IFileStorageManager
{
    Task<byte[]> GetImageContentAsync(string filename, CancellationToken cancellationToken);

    Task<FilePathInfo> SaveImageAsync(byte[] data, ImageContentTypes contentType, CancellationToken cancellationToken);

    void DeleteImage(string filename);
}

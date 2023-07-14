using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Utility.Abstractions;

public interface IFileService
{
    Task<byte[]> ReadFileAsync(string filename, FileTypes fileType, CancellationToken cancellationToken = default);

    Task<string> SaveDataAsync(byte[] data, FileTypes fileType, ImageRoles imageRole, CancellationToken cancellationToken = default);
}

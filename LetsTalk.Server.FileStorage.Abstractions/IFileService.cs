using LetsTalk.Server.FileStorage.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Abstractions;

public interface IFileService
{
    Task<byte[]> ReadFileAsync(string filename, FileTypes fileType, CancellationToken cancellationToken = default);

    Task<string> SaveDataAsync(
        byte[] data,
        FileTypes fileType,
        int width,
        int height,
        CancellationToken cancellationToken = default);

    Task SaveImageInfoAsync(
        string filename,
        int width,
        int height,
        CancellationToken cancellationToken = default);

    Task<ImageInfoModel> LoadImageInfoAsync(string filename, CancellationToken cancellationToken = default);

    void DeleteFile(string filename, FileTypes fileType);
}

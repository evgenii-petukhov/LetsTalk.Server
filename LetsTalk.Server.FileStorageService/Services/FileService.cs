using LetsTalk.Server.FileStorageService.Abstractions;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorageService.Services;

public class FileService : IFileService
{
    private readonly IFileNameGenerator _fileNameGenerator;

    public FileService(
        IFileNameGenerator fileNameGenerator)
    {
        _fileNameGenerator = fileNameGenerator;
    }

    public Task<byte[]> ReadFileAsync(string filename, FileTypes fileType, CancellationToken cancellationToken = default)
    {
        var imagePath = _fileNameGenerator.GetFilePath(filename, fileType);
        return File.ReadAllBytesAsync(imagePath, cancellationToken);
    }

    public Task<string> SaveDataAsync(byte[] data, FileTypes fileType, ImageRoles imageRole, CancellationToken cancellationToken = default)
    {
        return SaveDataWithUniqueNameAsyncRepeatableIfExistsAsync(data, () => _fileNameGenerator.Generate(fileType), cancellationToken);
    }

    public void DeleteFile(string filename, FileTypes fileType)
    {
        var path = _fileNameGenerator.GetFilePath(filename, fileType);
        File.Delete(path);
    }

    private static async Task<string> SaveDataWithUniqueNameAsyncRepeatableIfExistsAsync(
        byte[] data,
        Func<(string, string)> generateUniqueFilename,
        CancellationToken cancellationToken = default)
    {
        var (filename, filepath) = generateUniqueFilename.Invoke();
        try
        {
            await SaveDataWithUniqueNameAsync(data, filepath, cancellationToken);
        }
        catch (IOException)
        {
            (filename, filepath) = generateUniqueFilename.Invoke();
            await SaveDataWithUniqueNameAsync(data, filepath, cancellationToken);
        }
        return filename;
    }

    private static async Task SaveDataWithUniqueNameAsync(
        byte[] data,
        string filepath,
        CancellationToken cancellationToken = default)
    {
        using var stream = new FileStream(filepath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
        await stream.WriteAsync(data, cancellationToken);
    }
}

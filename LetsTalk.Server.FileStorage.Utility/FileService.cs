using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Utility;

public class FileService : IFileService
{
    private readonly IFileNameGenerator _fileNameGenerator;
    private readonly IFileStoragePathProvider _fileStoragePathProvider;

    public FileService(
        IFileNameGenerator fileNameGenerator,
        IFileStoragePathProvider fileStoragePathProvider)
    {
        _fileNameGenerator = fileNameGenerator;
        _fileStoragePathProvider = fileStoragePathProvider;
    }

    public Task<byte[]> ReadFileAsync(string filename, FileTypes fileType, CancellationToken cancellationToken = default)
    {
        var imagePath = _fileStoragePathProvider.GetFilePath(filename, fileType);
        return File.ReadAllBytesAsync(imagePath, cancellationToken);
    }

    public Task<string> SaveDataAsync(byte[] data, FileTypes fileType, ImageRoles imageRole, CancellationToken cancellationToken = default)
    {
        return SaveDataWithRetryAsync(data, () => _fileNameGenerator.Generate(fileType), cancellationToken);
    }

    private static async Task<string> SaveDataWithRetryAsync(
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

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

    public async Task<string?> SaveDataAsync(byte[] data, FileTypes fileType, ImageRoles imageRole, CancellationToken cancellationToken = default)
    {
        string filename;
        string filepath;
        (filename, filepath) = _fileNameGenerator.Generate(fileType);
        if (File.Exists(filepath))
        {
            (filename, filepath) = _fileNameGenerator.Generate(fileType);
        }
        await File.WriteAllBytesAsync(filepath, data, cancellationToken);
        return filename;
    }

    public void DeleteFile(string filename, FileTypes fileType)
    {
        var path = _fileNameGenerator.GetFilePath(filename, fileType);
        File.Delete(path);
    }
}

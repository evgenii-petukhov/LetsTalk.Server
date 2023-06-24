using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorageService.Abstractions;
using Microsoft.Extensions.Options;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorageService.Services;

public class FileNameGenerator : IFileNameGenerator
{
    private readonly FileStorageSettings _fileStorageSettings;

    public FileNameGenerator(
        IOptions<FileStorageSettings> fileStorageSettings)
    {
        _fileStorageSettings = fileStorageSettings.Value;
    }

    public (string, string) Generate(FileTypes fileType)
    {
        var filename = Guid.NewGuid().ToString();
        return (filename, GetFilePath(filename, fileType));
    }

    public string GetFilePath(string filename, FileTypes fileType)
    {
        var path = fileType switch
        {
            FileTypes.Image => Path.Combine(_fileStorageSettings.BasePath!, _fileStorageSettings.ImageFolder!, filename),
            _ => throw new NotImplementedException()
        };
        return Environment.ExpandEnvironmentVariables(path);
    }
}

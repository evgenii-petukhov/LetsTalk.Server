using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.FileStorage.Utility;

public class FileStoragePathProvider : IFileStoragePathProvider
{
    private readonly FileStorageSettings _fileStorageSettings;

    public FileStoragePathProvider(
        IOptions<FileStorageSettings> fileStorageSettings)
    {
        _fileStorageSettings = fileStorageSettings.Value;
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

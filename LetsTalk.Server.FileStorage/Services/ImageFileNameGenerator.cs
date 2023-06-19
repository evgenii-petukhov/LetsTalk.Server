using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.Abstractions;
using LetsTalk.Server.FileStorage.Models;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.FileStorage.Services;

public class ImageFileNameGenerator : IImageFileNameGenerator
{
    private readonly FileStorageSettings _fileStorageSettings;

    public ImageFileNameGenerator(
        IOptions<FileStorageSettings> fileStorageSettings)
    {
        _fileStorageSettings = fileStorageSettings.Value;
    }

    public FilePathInfo Generate()
    {
        var filename = Guid.NewGuid().ToString();
        return new FilePathInfo
        {
            FileName = filename,
            FullPath = GetImagePath(filename)
        };
    }

    public string GetImagePath(string filename)
    {
        var path = Path.Combine(_fileStorageSettings.BasePath!, _fileStorageSettings.ImageFolder!, filename);
        return Environment.ExpandEnvironmentVariables(path);
    }
}

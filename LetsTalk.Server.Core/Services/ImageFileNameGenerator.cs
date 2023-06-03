using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Core.Models;
using LetsTalk.Server.Persistence.Models;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Core.Services;

public class ImageFileNameGenerator : IImageFileNameGenerator
{
    private readonly IImageTypeService _imageTypeService;
    private readonly FileStorageSettings _fileStorageSettings;

    public ImageFileNameGenerator(
        IImageTypeService imageTypeService,
        IOptions<FileStorageSettings> fileStorageSettings)
    {
        _imageTypeService = imageTypeService;
        _fileStorageSettings = fileStorageSettings.Value;
    }

    public FilePathInfo Generate(ImageContentTypes contentType)
    {
        var extension = _imageTypeService.GetExtensionByImageType(contentType);
        var filename = Guid.NewGuid().ToString() + extension;
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

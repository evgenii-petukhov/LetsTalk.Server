using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
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

    public string GetFilename(ImageFileTypes imageFileType)
    {
        var extension = _imageTypeService.GetExtensionByImageType(imageFileType);
        var filename = Guid.NewGuid().ToString() + extension;
        filename = Path.Combine(_fileStorageSettings.BasePath!, _fileStorageSettings.ImageFolder!, filename);
        return Environment.ExpandEnvironmentVariables(filename);
    }
}

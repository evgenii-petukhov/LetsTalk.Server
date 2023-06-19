using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.FileStorage.Abstractions;
using LetsTalk.Server.FileStorage.Models;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.FileStorage.Services;

public class FileStorageManager : IFileStorageManager
{
    private readonly IImageFileNameGenerator _imageFileNameGenerator;
    private readonly IImageService _imageService;
    private readonly FileStorageSettings _fileStorageSettings;

    public FileStorageManager(
        IImageFileNameGenerator imageFileNameGenerator,
        IImageService imageService,
        IOptions<FileStorageSettings> fileStorageSettings)
    {
        _imageFileNameGenerator = imageFileNameGenerator;
        _imageService = imageService;
        _fileStorageSettings = fileStorageSettings.Value;
    }

    public Task<byte[]> GetImageContentAsync(string filename, CancellationToken cancellationToken)
    {
        var imagePath = _imageFileNameGenerator.GetImagePath(filename);
        return File.ReadAllBytesAsync(imagePath, cancellationToken);
    }

    public async Task<FilePathInfo> SaveImageAsync(byte[] data, CancellationToken cancellationToken)
    {
        var size = _imageService.GetImageSize(data);
        if (size.Width > _fileStorageSettings.AvatarMaxWidth || size.Height > _fileStorageSettings.AvatarMaxWidth)
        {
            throw new ImageSizeException("Image size exceeds max dimensions");
        }

        var filePathInfo = _imageFileNameGenerator.Generate();
        await File.WriteAllBytesAsync(filePathInfo.FullPath!, data, cancellationToken);
        return filePathInfo;
    }

    public void DeleteImage(string filename)
    {
        var path = _imageFileNameGenerator.GetImagePath(filename);
        File.Delete(path);
    }
}

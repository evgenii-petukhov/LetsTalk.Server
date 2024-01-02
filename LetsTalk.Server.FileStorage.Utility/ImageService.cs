using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.FileStorage.Utility.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Utility;

public class ImageService : IImageService
{
    private readonly IFileService _fileService;
    private readonly IFileStoragePathProvider _fileStoragePathProvider;

    public ImageService(
        IFileService fileService,
        IFileStoragePathProvider fileStoragePathProvider)
    {
        _fileService = fileService;
        _fileStoragePathProvider = fileStoragePathProvider;
    }

    public async Task<FetchImageResponse?> FetchImageAsync(string imageId, CancellationToken cancellationToken = default)
    {
        var filepath = _fileStoragePathProvider.GetFilePath(imageId, FileTypes.Image);
        var infoFilename = filepath + ".info";

        if (!File.Exists(infoFilename))
        {
            return null;
        }

        var imageInfo = await _fileService.LoadImageInfoAsync(infoFilename, cancellationToken);

        var content = await _fileService.ReadFileAsync(filepath, FileTypes.Image, cancellationToken);

        if (content.Length == 0)
        {
            return null;
        }

        return new FetchImageResponse
        {
            Content = content,
            Width = imageInfo?.Width ?? 0,
            Height = imageInfo?.Height ?? 0
        };
    }
}

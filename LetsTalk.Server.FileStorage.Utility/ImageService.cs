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

    public async Task<FetchImageResponse> FetchImageAsync(string imageId, CancellationToken cancellationToken = default)
    {
        var filename = _fileStoragePathProvider.GetFilePath(imageId, FileTypes.Image);
        var infoFilename = filename + ".info";
        var imageInfo = File.Exists(infoFilename)
            ? await _fileService.LoadImageInfoAsync(infoFilename, cancellationToken)
            : null;

        return new FetchImageResponse
        {
            Content = await _fileService.ReadFileAsync(filename, FileTypes.Image, cancellationToken),
            Width = imageInfo?.Width ?? 0,
            Height = imageInfo?.Height ?? 0
        };
    }
}

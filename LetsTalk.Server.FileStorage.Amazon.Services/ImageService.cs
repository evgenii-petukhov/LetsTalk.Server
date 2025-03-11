using LetsTalk.Server.FileStorage.AgnosticServices.Abstractions;
using LetsTalk.Server.FileStorage.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Amazon.Services;

public class ImageService(IFileService fileService) : IImageService
{
    private readonly IFileService _fileService = fileService;

    public async Task<FetchImageResponse?> FetchImageAsync(string imageId, CancellationToken cancellationToken = default)
    {
        var infoFilename = imageId + ".info";
        var imageInfo = await _fileService.LoadImageInfoAsync(infoFilename, cancellationToken);

        var content = await _fileService.ReadFileAsync(imageId, FileTypes.Image, cancellationToken);

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

using LetsTalk.Server.FileStorage.Abstractions;
using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.FileStorage.Service.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Service.Services;

public class ImageStorageService(IFileServiceResolver fileServiceResolver) : IImageStorageService
{
    private readonly IFileServiceResolver _fileServiceResolver = fileServiceResolver;

    public async Task<FetchImageResponse?> GetImageAsync(string imageId, FileStorageTypes fileStorageType, CancellationToken cancellationToken = default)
    {
        var fileService = _fileServiceResolver.Resolve(fileStorageType);

        var imageInfo = await fileService.LoadImageInfoAsync(imageId + ".info", cancellationToken);

        var content = await fileService.ReadFileAsync(imageId, FileTypes.Image, cancellationToken);

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

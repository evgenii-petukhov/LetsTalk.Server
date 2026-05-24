using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.FileStorage.Service.Models;

namespace LetsTalk.Server.FileStorage.Service.Services.Cache;

public class ImageStorageMemoryCacheService(IImageStorageService imageStorageService) : IImageStorageService, IImageStorageCacheManager
{
    private readonly IImageStorageService _imageStorageService = imageStorageService;

    public Task<FetchImageResponse?> GetImageAsync(string imageId, FileStorageTypes fileStorageType, CancellationToken cancellationToken = default)
    {
        return _imageStorageService.GetImageAsync(imageId, fileStorageType, cancellationToken);
    }

    public Task ClearAsync(string imageId)
    {
        return Task.CompletedTask;
    }
}

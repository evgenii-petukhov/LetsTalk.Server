using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.FileStorage.Utility.Abstractions.Models;

namespace LetsTalk.Server.FileStorage.Service.Services.Cache;

public class ImageMemoryCacheService : IImageService, IImageCacheManager
{
    private readonly IImageService _cacheService;

    public ImageMemoryCacheService(IImageService cacheService)
    {
        _cacheService = cacheService;
    }

    public Task<FetchImageResponse> FetchImageAsync(int imageId, bool useDimensions = false, CancellationToken cancellationToken = default)
    {
        return _cacheService.FetchImageAsync(imageId, useDimensions, cancellationToken);
    }

    public Task RemoveAsync(int imageId)
    {
        return Task.CompletedTask;
    }
}

using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.FileStorage.Utility.Abstractions.Models;

namespace LetsTalk.Server.FileStorage.Service.Services.Cache;

public class ImageMemoryCacheService(IImageService cacheService) : IImageService, IImageCacheManager
{
    private readonly IImageService _cacheService = cacheService;

    public Task<FetchImageResponse?> FetchImageAsync(string imageId, CancellationToken cancellationToken = default)
    {
        return _cacheService.FetchImageAsync(imageId, cancellationToken);
    }

    public Task RemoveAsync(string imageId)
    {
        return Task.CompletedTask;
    }
}

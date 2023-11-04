using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.FileStorage.Utility.Abstractions.Models;

namespace LetsTalk.Server.FileStorage.Service.Services.Cache;

public class MemoryCacheImageService : IImageService
{
    private readonly IImageService _cacheService;

    public MemoryCacheImageService(IImageService cacheService)
    {
        _cacheService = cacheService;
    }

    public Task<FetchImageResponse> FetchImageAsync(int imageId, bool useDimensions = false, CancellationToken cancellationToken = default)
    {
        return _cacheService.FetchImageAsync(imageId, useDimensions, cancellationToken);
    }
}

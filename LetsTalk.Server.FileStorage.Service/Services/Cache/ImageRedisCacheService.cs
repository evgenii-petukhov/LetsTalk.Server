using System.Text.Json;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.FileStorage.Utility.Abstractions.Models;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LetsTalk.Server.FileStorage.Service.Services.Cache;

public class ImageRedisCacheService : IImageService, IImageCacheManager
{
    private readonly bool _isActive;
    private readonly bool _isVolotile;

    private readonly TimeSpan _cacheLifeTimeInSeconds;
    private readonly int _imageSizeThresholdInBytes;

    private readonly IDatabase _database;
    private readonly IImageService _imageService;

    public ImageRedisCacheService(
        IConnectionMultiplexer сonnectionMultiplexer,
        IOptions<CachingSettings> cachingSettings,
        IImageService imageService)
    {
        _database = сonnectionMultiplexer.GetDatabase();
        _imageSizeThresholdInBytes = cachingSettings.Value.ImageSizeThresholdInBytes;
        _imageService = imageService;

        _isActive = cachingSettings.Value.ImagesCacheLifeTimeInSeconds != 0;
        _isVolotile = _isActive && cachingSettings.Value.ImagesCacheLifeTimeInSeconds > 0;

        if (_isVolotile)
        {
            _cacheLifeTimeInSeconds = TimeSpan.FromSeconds(cachingSettings.Value.ImagesCacheLifeTimeInSeconds);
        }
    }

    public async Task<FetchImageResponse> FetchImageAsync(string imageId, CancellationToken cancellationToken = default)
    {
        if (!_isActive)
        {
            return await _imageService.FetchImageAsync(imageId, cancellationToken);
        }

        var key = new RedisKey(GetImageKey(imageId));

        var cached = await _database.StringGetAsync(key);

        if (cached == RedisValue.Null)
        {
            var image = await _imageService.FetchImageAsync(imageId, cancellationToken);

            if (image?.Content?.Length < _imageSizeThresholdInBytes)
            {
                await _database.StringSetAsync(
                    key,
                    new RedisValue(JsonSerializer.Serialize(image)),
                    when: When.NotExists);

                if (_isVolotile)
                {
                    await _database.KeyExpireAsync(key, _cacheLifeTimeInSeconds);
                }
            }

            return image;
        }

        return JsonSerializer.Deserialize<FetchImageResponse>(cached!)!;
    }

    public Task RemoveAsync(string imageId)
    {
        return _isActive
            ? _database.KeyDeleteAsync(GetImageKey(imageId), CommandFlags.FireAndForget)
            : Task.CompletedTask;
    }

    private static string GetImageKey(string imageId)
    {
        return $"image:{imageId}";
    }
}

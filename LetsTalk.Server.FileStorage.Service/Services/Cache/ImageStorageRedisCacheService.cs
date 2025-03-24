using System.Text.Json;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.Persistence.Redis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.FileStorage.Service.Models;

namespace LetsTalk.Server.FileStorage.Service.Services.Cache;

public class ImageStorageRedisCacheService : IImageStorageService, IImageStorageCacheManager
{
    private readonly bool _isActive;
    private readonly bool _isVolotile;

    private readonly TimeSpan _cacheLifeTimeInSeconds;
    private readonly int _imageSizeThresholdInBytes;

    private readonly IDatabase _database;
    private readonly IImageStorageService _imageStorageService;

    public ImageStorageRedisCacheService(
        RedisConnection redisConnection,
        IOptions<CachingSettings> cachingSettings,
        IImageStorageService imageStorageService)
    {
        _database = redisConnection.Connection.GetDatabase();
        _imageSizeThresholdInBytes = cachingSettings.Value.ImageSizeThresholdInBytes;
        _imageStorageService = imageStorageService;

        _isActive = cachingSettings.Value.ImagesCacheLifeTimeInSeconds != 0;
        _isVolotile = _isActive && cachingSettings.Value.ImagesCacheLifeTimeInSeconds > 0;

        if (_isVolotile)
        {
            _cacheLifeTimeInSeconds = TimeSpan.FromSeconds(cachingSettings.Value.ImagesCacheLifeTimeInSeconds);
        }
    }

    public async Task<FetchImageResponse?> GetImageAsync(string imageId, FileStorageTypes fileStorageType, CancellationToken cancellationToken = default)
    {
        if (!_isActive)
        {
            return await _imageStorageService.GetImageAsync(imageId, fileStorageType, cancellationToken);
        }

        var key = new RedisKey(GetImageKey(imageId));

        var cached = await _database.StringGetAsync(key);

        if (cached == RedisValue.Null)
        {
            var image = await _imageStorageService.GetImageAsync(imageId, fileStorageType, cancellationToken);

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

    public Task ClearAsync(string imageId)
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

using System.Text.Json;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.FileStorage.Utility.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LetsTalk.Server.FileStorage.Service.Services.Cache;

public class RedisCacheImageService : IImageService
{
    private readonly TimeSpan _imagesCacheLifeTimeInSeconds;
    private readonly int _imageSizeThresholdInBytes;

    private readonly IDatabase _database;
    private readonly IImageService _imageService;

    public RedisCacheImageService(
        IConnectionMultiplexer сonnectionMultiplexer,
        IOptions<CachingSettings> cachingSettings,
        IImageService imageService)
    {
        _database = сonnectionMultiplexer.GetDatabase();
        _imagesCacheLifeTimeInSeconds = TimeSpan.FromSeconds(cachingSettings.Value.ImagesCacheLifeTimeInSeconds);
        _imageSizeThresholdInBytes = cachingSettings.Value.ImageSizeThresholdInBytes;
        _imageService = imageService;
    }

    public async Task<FetchImageResponse> FetchImageAsync(int imageId, bool useDimensions = false, CancellationToken cancellationToken = default)
    {
        var key = new RedisKey($"Images:{imageId}");

        var cached = await _database.StringGetAsync(key);

        if (cached == RedisValue.Null)
        {
            var image = await _imageService.FetchImageAsync(imageId, useDimensions, cancellationToken);

            if (image.Content!.Length < _imageSizeThresholdInBytes)
            {
                var isAvatar = image.ImageRole == ImageRoles.Avatar;

                await _database.StringSetAsync(
                    key,
                    new RedisValue(JsonSerializer.Serialize(image)),
                    isAvatar ? null : _imagesCacheLifeTimeInSeconds,
                    When.NotExists);

                if (!isAvatar)
                {
                    await _database.KeyExpireAsync(key, _imagesCacheLifeTimeInSeconds);
                }
            }

            return image;
        }

        return JsonSerializer.Deserialize<FetchImageResponse>(cached!)!;
    }
}

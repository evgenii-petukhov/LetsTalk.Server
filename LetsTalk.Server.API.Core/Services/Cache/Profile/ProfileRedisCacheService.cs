using System.Text.Json;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using LetsTalk.Server.Persistence.Redis;

namespace LetsTalk.Server.API.Core.Services.Cache.Profile;

public class ProfileRedisCacheService(
    RedisConnection redisConnection,
    IOptions<CachingSettings> cachingSettings,
    IProfileService profileService) : ProfileCacheServiceBase(profileService, cachingSettings), IProfileService, IProfileCacheManager
{
    private readonly IDatabase _database = redisConnection.Connection.GetDatabase();

    public async Task<ProfileDto> GetProfileAsync(string accountId, CancellationToken cancellationToken)
    {
        if (!_isActive)
        {
            return await _profileService.GetProfileAsync(accountId, cancellationToken);
        }

        var key = new RedisKey(GetProfileKey(accountId));

        var cachedProfile = await _database.StringGetAsync(key);

        if (cachedProfile == RedisValue.Null)
        {
            var profile = await _profileService.GetProfileAsync(accountId, cancellationToken);
            await _database.StringSetAsync(
                key,
                new RedisValue(JsonSerializer.Serialize(profile)),
                when: When.NotExists);

            if (_isVolotile)
            {
                await _database.KeyExpireAsync(key, _cacheLifeTimeInSeconds);
            }

            return profile;
        }

        return JsonSerializer.Deserialize<ProfileDto>(cachedProfile!)!;
    }

    public async Task ClearAsync(string accountId)
    {
        if (_isActive)
        {
            await _database.KeyDeleteAsync(GetProfileKey(accountId), CommandFlags.FireAndForget);
        }
    }
}

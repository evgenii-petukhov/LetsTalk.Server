using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Persistence.Redis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LetsTalk.Server.API.Core.Services.Cache.IceServerConfiguration;

public class IceServerConfigurationRedisCacheService(
    RedisConnection redisConnection,
    IIceServerConfigurationService iceServerConfigurationService,
    IOptions<CloudflareSettings> options) : IceServerConfigurationCacheServiceBase(
        iceServerConfigurationService,
        options), IIceServerConfigurationService
{
    private readonly IDatabase _database = redisConnection.Connection.GetDatabase();

    public async Task<string> GetIceServerConfigurationAsync(CancellationToken cancellationToken)
    {
        var key = new RedisKey(Key);

        var cachedConfiguration = await _database.StringGetAsync(key);

        if (cachedConfiguration == RedisValue.Null)
        {
            var configuration = await IceServerConfigurationService.GetIceServerConfigurationAsync(cancellationToken);
            await _database.StringSetAsync(
                key,
                new RedisValue(configuration),
                when: When.NotExists);

            await _database.KeyExpireAsync(key, CacheLifeTimeInSeconds);

            return configuration;
        }

        return cachedConfiguration!;
    }
}

using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.API.Core.Abstractions;
using Microsoft.Extensions.Options;
using LetsTalk.Server.Persistence.Redis;
using StackExchange.Redis;
using LetsTalk.Server.API.Core.Services.Cache.LoginCodes;
using System.Globalization;

namespace LetsTalk.Server.API.Core.Services.Cache.Messages;

public class LoginCodeRedisCacheService(
    RedisConnection redisConnection,
    ILoginCodeGenerator loginCodeGenerator,
    IOptions<CachingSettings> cachingSettings) : LoginCodeCacheServiceBase(cachingSettings), ILoginCodeCacheService
{
    private readonly ILoginCodeGenerator _generator = loginCodeGenerator;
    private readonly IDatabase _database = redisConnection.Connection.GetDatabase();

    public async Task<(int, bool, TimeSpan)> GenerateCodeAsync(string email)
    {
        var key = new RedisKey(GetLoginCodeKey(email));
        var code = _generator.GenerateCode();

        var isCreated = await _database.StringSetAsync(key, new RedisValue(code.ToString(CultureInfo.InvariantCulture)), CacheLifeTimeInSeconds, when: When.NotExists);
        var ttl = await _database.KeyTimeToLiveAsync(key);
        return (code, isCreated, ttl ?? CacheLifeTimeInSeconds);
    }

    public async Task<bool> ValidateCodeAsync(string email, int code)
    {
        var key = new RedisKey(GetLoginCodeKey(email));
        var value = await _database.StringGetAsync(key);

        return string.Equals(value, code.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
    }
}

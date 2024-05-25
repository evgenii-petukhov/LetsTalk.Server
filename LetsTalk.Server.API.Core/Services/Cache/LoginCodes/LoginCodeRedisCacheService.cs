using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LetsTalk.Server.Core.Services.Cache.Messages;

public class LoginCodeRedisCacheService(
    IConnectionMultiplexer сonnectionMultiplexer,
    ILoginCodeGenerator loginCodeGenerator,
    IOptions<CachingSettings> cachingSettings) : LoginCodeCacheServiceBase(cachingSettings), ILoginCodeCacheService
{
    private readonly ILoginCodeGenerator _generator = loginCodeGenerator;
    private readonly IDatabase _database = сonnectionMultiplexer.GetDatabase();

    public async Task<(int, bool)> GenerateCodeAsync(string email)
    {
        var key = new RedisKey(GetLoginCodeKey(email));
        var code = _generator.GenerateCode();

        var isCreated = await _database.StringSetAsync(key, new RedisValue(code.ToString()), _cacheLifeTimeInSeconds, when: When.NotExists);

        return (code, isCreated);
    }

    public async Task<bool> ValidateCodeAsync(string email, int code)
    {
        var key = new RedisKey(GetLoginCodeKey(email));
        var value = await _database.StringGetAsync(key);

        return string.Equals(value, code.ToString(), StringComparison.Ordinal);
    }
}

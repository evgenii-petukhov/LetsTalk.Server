using LetsTalk.Server.Authentication.Abstractions;
using StackExchange.Redis;
using System.Text.Json;

namespace LetsTalk.Server.Authentication.Services.Cache;

public class RedisCacheTokenService : CacheTokenServiceBase, IJwtCacheService
{
    private readonly IJwtStorageService _jwtStorageService;
    private readonly IDatabase _database;

    public RedisCacheTokenService(
        IJwtStorageService jwtStorageService,
        IConnectionMultiplexer сonnectionMultiplexer)
    {
        _jwtStorageService = jwtStorageService;
        _database = сonnectionMultiplexer.GetDatabase();
    }

    public async Task<int?> GetAccountIdAsync(string? token)
    {
        if (token == null)
        {
            return null;
        }

        var key = new RedisKey(GetTokenKey(token!));

        var cachedAccountId = await _database.StringGetAsync(key);

        if (cachedAccountId == RedisValue.Null)
        {
            var storedToken = await _jwtStorageService.GetStoredTokenAsync(token);

            await _database.StringSetAsync(
                key,
                new RedisValue(JsonSerializer.Serialize(storedToken!.AccountId)),
                storedToken!.ValidTo - DateTime.Now,
            When.NotExists);

            await _database.KeyExpireAsync(key, storedToken.ValidTo);

            return storedToken.AccountId;
        }

        return JsonSerializer.Deserialize<int?>(cachedAccountId!)!;
    }

    public async Task<string> GenerateAsync(int accountId)
    {
        var storedToken = await _jwtStorageService.GenerateAsync(accountId);

        var key = new RedisKey(GetTokenKey(storedToken.Token!));

        await _database.StringSetAsync(
                key,
                new RedisValue(JsonSerializer.Serialize(storedToken.AccountId)),
                storedToken.ValidTo - DateTime.Now);

        return storedToken.Token!;
    }
}

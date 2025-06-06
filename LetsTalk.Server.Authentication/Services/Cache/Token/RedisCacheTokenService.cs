﻿using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Persistence.Redis;
using StackExchange.Redis;
using System.Text.Json;

namespace LetsTalk.Server.Authentication.Services.Cache.Token;

public class RedisCacheTokenService(
    IJwtStorageService jwtStorageService,
    RedisConnection redisConnection) : CacheTokenServiceBase, IJwtCacheService
{
    private readonly IJwtStorageService _jwtStorageService = jwtStorageService;
    private readonly IDatabase _database = redisConnection.Connection.GetDatabase();

    public async Task<string?> GetAccountIdAsync(string? token)
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

        return JsonSerializer.Deserialize<string?>(cachedAccountId!)!;
    }

    public async ValueTask<string> GenerateAsync(string accountId)
    {
        var storedToken = _jwtStorageService.Generate(accountId);

        var key = new RedisKey(GetTokenKey(storedToken.Token!));

        await _database.StringSetAsync(
                key,
                new RedisValue(JsonSerializer.Serialize(storedToken.AccountId)),
                storedToken.ValidTo - DateTime.Now);

        return storedToken.Token!;
    }
}

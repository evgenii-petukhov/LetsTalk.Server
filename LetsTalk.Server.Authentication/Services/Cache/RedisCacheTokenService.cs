using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Authentication.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace LetsTalk.Server.Authentication.Services.Cache;

public class RedisCacheTokenService : IJwtStorageService
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

    public async Task<StoredToken?> GetStoredTokenAsync(string? token)
    {
        if (token == null)
        {
            return null;
        }

        var key = new RedisKey(GetTokenKey(token!));

        var cached = await _database.StringGetAsync(key);

        if (cached == RedisValue.Null)
        {
            var storedToken = await _jwtStorageService.GetStoredTokenAsync(token);

            await _database.StringSetAsync(
                key,
                new RedisValue(JsonSerializer.Serialize(storedToken)),
                storedToken!.ValidTo - DateTime.Now,
            When.NotExists);

            await _database.KeyExpireAsync(key, storedToken.ValidTo);

            return storedToken;
        }

        return JsonSerializer.Deserialize<StoredToken>(cached!)!;
    }

    public async Task<StoredToken> GenerateJwtToken(int accountId)
    {
        var storedToken = await _jwtStorageService.GenerateJwtToken(accountId);

        var key = new RedisKey(GetTokenKey(storedToken.Token!));

        await _database.StringSetAsync(
                key,
                new RedisValue(JsonSerializer.Serialize(storedToken)),
                storedToken.ValidTo - DateTime.Now);

        return storedToken;
    }

    private static string GetTokenKey(string token)
    {
        return $"Jwt:{token}";
    }
}

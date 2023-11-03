using System.Text.Json;
using LetsTalk.Server.Caching.Abstractions;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Dto.Models;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LetsTalk.Server.Caching;

public class RedisCacheService : ICacheService
{
    private readonly TimeSpan _messagesCacheLifeTimeInSeconds;
    private readonly TimeSpan _contactsCacheLifeTimeInSeconds;

    private readonly IDatabase _database;

    public RedisCacheService(
        IConnectionMultiplexer сonnectionMultiplexer,
        IOptions<CachingSettings> cachingSettings)
    {
        _database = сonnectionMultiplexer.GetDatabase();
        _messagesCacheLifeTimeInSeconds = TimeSpan.FromSeconds(cachingSettings.Value.MessagesCacheLifeTimeInSeconds);
        _contactsCacheLifeTimeInSeconds = TimeSpan.FromSeconds(cachingSettings.Value.ContactsCacheLifeTimeInSeconds);
    }

    public async Task<List<MessageDto>>? GetOrAddMessagesAsync(int senderId, int recipientId, int pageIndex, Func<Task<List<MessageDto>>> factory)
    {
        var key = new RedisKey($"Messages:{senderId}:{recipientId}");
        await _database.KeyExpireAsync(key, _messagesCacheLifeTimeInSeconds);

        var cachedMessages = await _database.HashGetAsync(key, new RedisValue(pageIndex.ToString()));

        if (cachedMessages == RedisValue.Null)
        {
            var messageDtos = await factory();
            await _database.HashSetAsync(
                key,
                new RedisValue(pageIndex.ToString()),
                new RedisValue(JsonSerializer.Serialize(messageDtos)),
                When.NotExists);

            return messageDtos;
        }

        return JsonSerializer.Deserialize<List<MessageDto>>(cachedMessages!)!;
    }

    public Task RemoveMessages(int senderId, int recipientId)
    {
        return _database.KeyDeleteAsync(new RedisKey($"Messages:{senderId}:{recipientId}"));
    }

    public async Task<List<AccountDto>>? GetOrAddAccountsAsync(int accountId, Func<Task<List<AccountDto>>> factory)
    {
        var key = new RedisKey($"Accounts:{accountId}");
        await _database.KeyExpireAsync(key, _contactsCacheLifeTimeInSeconds);

        var cachedAccounts = await _database.StringGetAsync(key);

        if (cachedAccounts == RedisValue.Null)
        {
            var accountDtos = await factory();
            await _database.StringSetAsync(
                key,
                new RedisValue(JsonSerializer.Serialize(accountDtos)),
                _contactsCacheLifeTimeInSeconds,
                When.NotExists);

            return accountDtos;
        }

        return JsonSerializer.Deserialize<List<AccountDto>>(cachedAccounts!)!;
    }
}

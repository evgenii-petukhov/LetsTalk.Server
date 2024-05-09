using System.Text.Json;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LetsTalk.Server.Core.Services.Cache.Chats;

public class ChatRedisCacheService(
    IConnectionMultiplexer сonnectionMultiplexer,
    IOptions<CachingSettings> cachingSettings,
    IChatService chatService) : ChatCacheServiceBase(chatService, cachingSettings), IChatService, IChatCacheManager
{
    private readonly IDatabase _database = сonnectionMultiplexer.GetDatabase();

    public async Task<IReadOnlyList<ChatDto>> GetChatsAsync(string accountId, CancellationToken cancellationToken)
    {
        if (!_isActive)
        {
            return await _chatService.GetChatsAsync(accountId, cancellationToken);
        }

        var key = new RedisKey(GetChatsKey(accountId));

        var cachedAccounts = await _database.StringGetAsync(key);

        if (cachedAccounts == RedisValue.Null)
        {
            var accountDtos = await _chatService.GetChatsAsync(accountId, cancellationToken);
            await _database.StringSetAsync(
                key,
                new RedisValue(JsonSerializer.Serialize(accountDtos)),
                when: When.NotExists);

            if (_isVolotile)
            {
                await _database.KeyExpireAsync(key, _cacheLifeTimeInSeconds);
            }

            return accountDtos;
        }

        return JsonSerializer.Deserialize<List<ChatDto>>(cachedAccounts!)!;
    }

    public async Task RemoveAsync(string accountId)
    {
        if (_isActive)
        {
            await _database.KeyDeleteAsync(GetChatsKey(accountId), CommandFlags.FireAndForget);
        }
    }
}

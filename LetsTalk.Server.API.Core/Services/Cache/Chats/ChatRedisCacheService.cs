using System.Text.Json;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using LetsTalk.Server.Persistence.Redis;

namespace LetsTalk.Server.API.Core.Services.Cache.Chats;

public class ChatRedisCacheService(
    RedisConnection redisConnection,
    IOptions<CachingSettings> cachingSettings,
    IChatService chatService) : ChatCacheServiceBase(chatService, cachingSettings), IChatService, IChatCacheManager
{
    private readonly IDatabase _database = redisConnection.Connection.GetDatabase();

    public async Task<IReadOnlyList<ChatDto>> GetChatsAsync(string accountId, CancellationToken cancellationToken)
    {
        if (!IsActive)
        {
            return await ChatService.GetChatsAsync(accountId, cancellationToken);
        }

        var key = new RedisKey(GetChatsKey(accountId));

        var cachedAccounts = await _database.StringGetAsync(key);

        if (cachedAccounts == RedisValue.Null)
        {
            var accountDtos = await ChatService.GetChatsAsync(accountId, cancellationToken);
            await _database.StringSetAsync(
                key,
                new RedisValue(JsonSerializer.Serialize(accountDtos)),
                when: When.NotExists);

            if (IsVolatile)
            {
                await _database.KeyExpireAsync(key, CacheLifeTimeInSeconds);
            }

            return accountDtos;
        }

        return JsonSerializer.Deserialize<List<ChatDto>>(cachedAccounts!)!;
    }

    public async Task ClearAsync(string accountId)
    {
        if (IsActive)
        {
            await _database.KeyDeleteAsync(GetChatsKey(accountId), CommandFlags.FireAndForget);
        }
    }

    public Task<bool> IsChatIdValidAsync(string chatId, CancellationToken cancellationToken)
    {
        return ChatService.IsChatIdValidAsync(chatId, cancellationToken);
    }
}

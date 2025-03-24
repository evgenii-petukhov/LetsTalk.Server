using System.Text.Json;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.API.Core.Abstractions;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using LetsTalk.Server.Persistence.Redis;
using System.Globalization;
using LetsTalk.Server.Persistence.AgnosticServices.Models;

namespace LetsTalk.Server.API.Core.Services.Cache.Messages;

public class MessageRedisCacheService(
    RedisConnection redisConnection,
    IOptions<CachingSettings> cachingSettings,
    IMessageService messageService) : MessageCacheServiceBase(messageService, cachingSettings), IMessageService, IMessageCacheManager
{
    private readonly IDatabase _database = redisConnection.Connection.GetDatabase();

    public async Task<IReadOnlyList<MessageServiceModel>> GetPagedAsync(string chatId, int pageIndex, int messagesPerPage, CancellationToken cancellationToken)
    {
        if (!IsActive)
        {
            return await MessageService.GetPagedAsync(
                chatId,
                pageIndex,
                messagesPerPage,
                cancellationToken);
        }

        var key = new RedisKey(pageIndex == 0
            ? GetFirstMessagePageKey(chatId)
            : GetMessagePageKey(chatId));

        var cachedMessages = await _database.HashGetAsync(key, new RedisValue(pageIndex.ToString(CultureInfo.InvariantCulture)));

        if (cachedMessages == RedisValue.Null)
        {
            var messageDtos = await MessageService.GetPagedAsync(
                chatId,
                pageIndex,
                messagesPerPage,
                cancellationToken);

            await _database.HashSetAsync(
                key,
                new RedisValue(pageIndex.ToString(CultureInfo.InvariantCulture)),
                new RedisValue(JsonSerializer.Serialize(messageDtos)),
                When.NotExists);

            if (IsVolotile && pageIndex > 0)
            {
                await _database.KeyExpireAsync(key, CacheLifeTimeInSeconds);
            }

            return messageDtos;
        }

        return JsonSerializer.Deserialize<List<MessageServiceModel>>(cachedMessages!)!;
    }

    public Task ClearAsync(string chatId)
    {
        return IsActive
            ? Task.WhenAll(
                _database.KeyDeleteAsync(GetMessagePageKey(chatId), CommandFlags.FireAndForget),
                _database.KeyDeleteAsync(GetFirstMessagePageKey(chatId), CommandFlags.FireAndForget))
            : Task.CompletedTask;
    }
}

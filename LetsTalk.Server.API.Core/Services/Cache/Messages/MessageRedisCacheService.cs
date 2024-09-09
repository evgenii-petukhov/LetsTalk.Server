using System.Text.Json;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LetsTalk.Server.API.Core.Services.Cache.Messages;

public class MessageRedisCacheService(
    IConnectionMultiplexer сonnectionMultiplexer,
    IOptions<CachingSettings> cachingSettings,
    IMessageService messageService) : MessageCacheServiceBase(messageService, cachingSettings), IMessageService, IMessageCacheManager
{
    private readonly IDatabase _database = сonnectionMultiplexer.GetDatabase();

    public async Task<IReadOnlyList<MessageServiceModel>> GetPagedAsync(string chatId, int pageIndex, int messagesPerPage, CancellationToken cancellationToken)
    {
        if (!_isActive)
        {
            return await _messageService.GetPagedAsync(
                chatId,
                pageIndex,
                messagesPerPage,
                cancellationToken);
        }

        var key = new RedisKey(pageIndex == 0
            ? GetFirstMessagePageKey(chatId)
            : GetMessagePageKey(chatId));

        var cachedMessages = await _database.HashGetAsync(key, new RedisValue(pageIndex.ToString()));

        if (cachedMessages == RedisValue.Null)
        {
            var messageDtos = await _messageService.GetPagedAsync(
                chatId,
                pageIndex,
                messagesPerPage,
                cancellationToken);

            await _database.HashSetAsync(
                key,
                new RedisValue(pageIndex.ToString()),
                new RedisValue(JsonSerializer.Serialize(messageDtos)),
                When.NotExists);

            if (_isVolotile && pageIndex > 0)
            {
                await _database.KeyExpireAsync(key, _cacheLifeTimeInSeconds);
            }

            return messageDtos;
        }

        return JsonSerializer.Deserialize<List<MessageServiceModel>>(cachedMessages!)!;
    }

    public Task ClearAsync(string chatId)
    {
        return _isActive
            ? Task.WhenAll(
                _database.KeyDeleteAsync(GetMessagePageKey(chatId), CommandFlags.FireAndForget),
                _database.KeyDeleteAsync(GetFirstMessagePageKey(chatId), CommandFlags.FireAndForget))
            : Task.CompletedTask;
    }
}

﻿using System.Text.Json;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LetsTalk.Server.Core.Services.Cache.Messages;

public class MessageRedisCacheService : MessageCacheServiceBase, IMessageService, IMessageCacheManager
{
    private readonly IDatabase _database;

    public MessageRedisCacheService(
        IConnectionMultiplexer сonnectionMultiplexer,
        IOptions<CachingSettings> cachingSettings,
        IMessageService messageService) : base(messageService, cachingSettings)
    {
        _database = сonnectionMultiplexer.GetDatabase();
    }

    public async Task<List<MessageDto>> GetPagedAsync(
        int senderId,
        int recipientId,
        int pageIndex,
        int messagesPerPage,
        CancellationToken cancellationToken)
    {
        if (!_isActive)
        {
            return await _messageService.GetPagedAsync(
                senderId,
                recipientId,
                pageIndex,
                messagesPerPage,
                cancellationToken);
        }

        var key = new RedisKey(pageIndex == 0
            ? GetFirstMessagePageKey(senderId, recipientId)
            : GetMessagePageKey(senderId, recipientId));

        var cachedMessages = await _database.HashGetAsync(key, new RedisValue(pageIndex.ToString()));

        if (cachedMessages == RedisValue.Null)
        {
            var messageDtos = await _messageService.GetPagedAsync(
                senderId,
                recipientId,
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

        return JsonSerializer.Deserialize<List<MessageDto>>(cachedMessages!)!;
    }

    public Task RemoveAsync(int senderId, int recipientId)
    {
        return _isActive
            ? Task.WhenAll(
                _database.KeyDeleteAsync(GetMessagePageKey(senderId, recipientId), CommandFlags.FireAndForget),
                _database.KeyDeleteAsync(GetMessagePageKey(recipientId, senderId), CommandFlags.FireAndForget),
                _database.KeyDeleteAsync(GetFirstMessagePageKey(senderId, recipientId), CommandFlags.FireAndForget),
                _database.KeyDeleteAsync(GetFirstMessagePageKey(recipientId, senderId), CommandFlags.FireAndForget))
            : Task.CompletedTask;
    }
}
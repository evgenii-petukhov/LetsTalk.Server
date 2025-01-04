using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace LetsTalk.Server.API.Core.Services.Cache.Messages;

public class MessageMemoryCacheService(
    IMemoryCache memoryCache,
    IOptions<CachingSettings> cachingSettings,
    IMessageService messageService) : MessageCacheServiceBase(messageService, cachingSettings), IMessageService, IMessageCacheManager
{
    private readonly IMemoryCache _memoryCache = memoryCache;

    public Task<IReadOnlyList<MessageServiceModel>> GetPagedAsync(string chatId, int pageIndex, int messagesPerPage, CancellationToken cancellationToken)
    {
        if (!IsActive)
        {
            return MessageService.GetPagedAsync(
                chatId,
                pageIndex,
                messagesPerPage,
                cancellationToken);
        }

        return _memoryCache.GetOrCreateAsync(GetMessagePageKey(chatId), cacheEntry =>
        {
            if (IsVolotile && pageIndex > 0)
            {
                cacheEntry.SetAbsoluteExpiration(CacheLifeTimeInSeconds);
            }
            var dict = new ConcurrentDictionary<int, Task<IReadOnlyList<MessageServiceModel>>>();
            return dict.GetOrAdd(pageIndex, _ => MessageService.GetPagedAsync(
                chatId,
                pageIndex,
                messagesPerPage,
                cancellationToken));
        })!;
    }

    public Task ClearAsync(string chatId)
    {
        if (IsActive)
        {
            _memoryCache.Remove(GetMessagePageKey(chatId));
            _memoryCache.Remove(GetFirstMessagePageKey(chatId));
        }

        return Task.CompletedTask;
    }
}

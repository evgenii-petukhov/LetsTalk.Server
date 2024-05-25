using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace LetsTalk.Server.Core.Services.Cache.Messages;

public class MessageMemoryCacheService(
    IMemoryCache memoryCache,
    IOptions<CachingSettings> cachingSettings,
    IMessageService messageService) : MessageCacheServiceBase(messageService, cachingSettings), IMessageService, IMessageCacheManager
{
    private readonly IMemoryCache _memoryCache = memoryCache;

    public Task<IReadOnlyList<MessageServiceModel>> GetPagedAsync(string chatId, int pageIndex, int messagesPerPage, CancellationToken cancellationToken)
    {
        if (!_isActive)
        {
            return _messageService.GetPagedAsync(
                chatId,
                pageIndex,
                messagesPerPage,
                cancellationToken);
        }

        return _memoryCache.GetOrCreateAsync(GetMessagePageKey(chatId), cacheEntry =>
        {
            if (_isVolotile && pageIndex > 0)
            {
                cacheEntry.SetAbsoluteExpiration(_cacheLifeTimeInSeconds);
            }
            var dict = new ConcurrentDictionary<int, Task<IReadOnlyList<MessageServiceModel>>>();
            return dict.GetOrAdd(pageIndex, _ => _messageService.GetPagedAsync(
                chatId,
                pageIndex,
                messagesPerPage,
                cancellationToken));
        })!;
    }

    public Task RemoveAsync(string chatId)
    {
        if (_isActive)
        {
            _memoryCache.Remove(GetMessagePageKey(chatId));
            _memoryCache.Remove(GetFirstMessagePageKey(chatId));
        }

        return Task.CompletedTask;
    }
}

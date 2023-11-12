using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace LetsTalk.Server.Core.Services.Cache.Messages;

public class MessageMemoryCacheService : MessageCacheServiceBase, IMessageService, IMessageCacheManager
{
    private readonly IMemoryCache _memoryCache;

    public MessageMemoryCacheService(
        IMemoryCache memoryCache,
        IOptions<CachingSettings> cachingSettings,
        IMessageService messageService) : base(messageService, cachingSettings)
    {
        _memoryCache = memoryCache;
    }

    public Task<List<MessageDto>> GetPagedAsync(int senderId, int recipientId, int pageIndex, int messagesPerPage, CancellationToken cancellationToken)
    {
        if (!_isActive)
        {
            return _messageService.GetPagedAsync(
                senderId,
                recipientId,
                pageIndex,
                messagesPerPage,
                cancellationToken);
        }

        return _memoryCache.GetOrCreateAsync(GetMessagePageKey(senderId, recipientId), cacheEntry =>
        {
            if (_isVolotile && pageIndex > 0)
            {
                cacheEntry.SetAbsoluteExpiration(_cacheLifeTimeInSeconds);
            }
            var dict = new ConcurrentDictionary<int, Task<List<MessageDto>>>();
            return dict.GetOrAdd(pageIndex, _ => _messageService.GetPagedAsync(
                senderId,
                recipientId,
                pageIndex,
                messagesPerPage,
                cancellationToken));
        })!;
    }

    public Task RemoveAsync(int senderId, int recipientId)
    {
        if (_isActive)
        {
            _memoryCache.Remove(GetMessagePageKey(senderId, recipientId));
            _memoryCache.Remove(GetMessagePageKey(recipientId, senderId));
            _memoryCache.Remove(GetFirstMessagePageKey(senderId, recipientId));
            _memoryCache.Remove(GetFirstMessagePageKey(recipientId, senderId));
        }

        return Task.CompletedTask;
    }
}

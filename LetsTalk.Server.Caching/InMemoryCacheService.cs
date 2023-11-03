using System.Collections.Concurrent;
using LetsTalk.Server.Dto.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using LetsTalk.Server.Caching.Abstractions;
using LetsTalk.Server.Configuration.Models;

namespace LetsTalk.Server.Caching;

public class InMemoryCacheService: ICacheService
{
    private readonly TimeSpan _contactsCacheLifeTimeInSeconds;

    private readonly ConcurrentDictionary<MessageCacheKey, ConcurrentDictionary<int, Task<List<MessageDto>>>> _cache = new();
    private readonly IMemoryCache _memoryCache;

    public InMemoryCacheService(
        IMemoryCache memoryCache,
        IOptions<CachingSettings> cachingSettings)
    {
        _memoryCache = memoryCache;
        _contactsCacheLifeTimeInSeconds = TimeSpan.FromSeconds(cachingSettings.Value.ContactsCacheLifeTimeInSeconds);
    }

    public Task<List<MessageDto>> GetOrAddMessagesAsync(int senderId, int recipientId, int pageIndex, Func<Task<List<MessageDto>>> factory)
    {
        var key = new MessageCacheKey
        {
            SenderId = senderId,
            RecipientId = recipientId
        };

        var dict = _cache.GetOrAdd(key, _ => new ConcurrentDictionary<int, Task<List<MessageDto>>>());

        return dict.GetOrAdd(pageIndex, _ => factory());
    }

    public Task<List<AccountDto>> GetOrAddAccountsAsync(int accountId, Func<Task<List<AccountDto>>> factory)
    {
        return _memoryCache.GetOrCreateAsync(accountId, cacheEntry =>
        {
            cacheEntry.SetAbsoluteExpiration(_contactsCacheLifeTimeInSeconds);
            return factory();
        })!;
    }

    public Task<ImageCacheEntry> GetOrAddImageAsync(int imageId, Func<Task<ImageCacheEntry>> factory)
    {
        return factory();
    }

    public Task RemoveMessagesAsync(int senderId, int recipientId)
    {
        var key = new MessageCacheKey
        {
            SenderId = senderId,
            RecipientId = recipientId
        };

        _cache.TryRemove(key, out _);

        return Task.CompletedTask;
    }
}

using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Core.Services.Cache.Contacts;

public class ChatMemoryCacheService(
    IMemoryCache memoryCache,
    IOptions<CachingSettings> cachingSettings,
    IChatService accountService) : ChatCacheServiceBase(accountService, cachingSettings), IChatService
{
    private readonly IMemoryCache _memoryCache = memoryCache;

    public Task<List<ChatDto>> GetChatsAsync(string accountId, CancellationToken cancellationToken)
    {
        return _isActive
            ? _memoryCache.GetOrCreateAsync(GetContactsKey(accountId), cacheEntry =>
            {
                if (_isVolotile)
                {
                    cacheEntry.SetAbsoluteExpiration(_cacheLifeTimeInSeconds);
                }

                return _accountService.GetChatsAsync(accountId, cancellationToken);
            })!
            : _accountService.GetChatsAsync(accountId, cancellationToken);
    }
}

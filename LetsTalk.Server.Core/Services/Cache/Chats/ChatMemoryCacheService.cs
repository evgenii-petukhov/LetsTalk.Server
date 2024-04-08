using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Core.Services.Cache.Chats;

public class ChatMemoryCacheService(
    IMemoryCache memoryCache,
    IOptions<CachingSettings> cachingSettings,
    IChatService chatService) : ChatCacheServiceBase(chatService, cachingSettings), IChatService
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

                return _chatService.GetChatsAsync(accountId, cancellationToken);
            })!
            : _chatService.GetChatsAsync(accountId, cancellationToken);
    }
}

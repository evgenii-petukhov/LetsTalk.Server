using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Core.Services.Cache.Chats;

public class ChatMemoryCacheService(
    IMemoryCache memoryCache,
    IOptions<CachingSettings> cachingSettings,
    IChatService chatService) : ChatCacheServiceBase(chatService, cachingSettings), IChatService, IChatCacheManager
{
    private readonly IMemoryCache _memoryCache = memoryCache;

    public Task<IReadOnlyList<ChatDto>> GetChatsAsync(string accountId, CancellationToken cancellationToken)
    {
        return IsActive
            ? _memoryCache.GetOrCreateAsync(GetChatsKey(accountId), cacheEntry =>
            {
                if (IsVolotile)
                {
                    cacheEntry.SetAbsoluteExpiration(CacheLifeTimeInSeconds);
                }

                return ChatService.GetChatsAsync(accountId, cancellationToken);
            })!
            : ChatService.GetChatsAsync(accountId, cancellationToken);
    }

    public Task ClearAsync(string accountId)
    {
        if (IsActive)
        {
            _memoryCache.Remove(GetChatsKey(accountId));
        }

        return Task.CompletedTask;
    }
}

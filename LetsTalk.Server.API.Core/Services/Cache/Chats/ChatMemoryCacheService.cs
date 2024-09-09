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
        return _isActive
            ? _memoryCache.GetOrCreateAsync(GetChatsKey(accountId), cacheEntry =>
            {
                if (_isVolotile)
                {
                    cacheEntry.SetAbsoluteExpiration(_cacheLifeTimeInSeconds);
                }

                return _chatService.GetChatsAsync(accountId, cancellationToken);
            })!
            : _chatService.GetChatsAsync(accountId, cancellationToken);
    }

    public Task ClearAsync(string accountId)
    {
        if (_isActive)
        {
            _memoryCache.Remove(GetChatsKey(accountId));
        }

        return Task.CompletedTask;
    }
}

using LetsTalk.Server.Authentication.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace LetsTalk.Server.Authentication.Services.Cache;

public class MemoryCacheTokenService : CacheTokenServiceBase, IJwtCacheService
{
    private readonly IJwtStorageService _jwtStorageService;
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheTokenService(
        IJwtStorageService jwtStorageService,
        IMemoryCache memoryCache)
    {
        _jwtStorageService = jwtStorageService;
        _memoryCache = memoryCache;
    }

    public async Task<string?> GetAccountIdAsync(string? token)
    {
        return token == null
            ? null
            : await _memoryCache.GetOrCreateAsync(GetTokenKey(token), async cacheEntry =>
            {
                var storedToken = await _jwtStorageService.GetStoredTokenAsync(token);
                cacheEntry.AbsoluteExpiration = storedToken!.ValidTo;
                return storedToken.AccountId;
            });
    }

    public async Task<string> GenerateAsync(string accountId)
    {
        var storedToken = await _jwtStorageService.GenerateAsync(accountId);
        _memoryCache.Set(GetTokenKey(storedToken.Token!), storedToken.AccountId, storedToken.ValidTo);
        return storedToken.Token!;
    }
}

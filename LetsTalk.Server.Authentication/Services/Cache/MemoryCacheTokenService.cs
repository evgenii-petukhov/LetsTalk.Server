using LetsTalk.Server.Authentication.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace LetsTalk.Server.Authentication.Services.Cache;

public class MemoryCacheTokenService : IJwtCacheService
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

    public async Task<int?> GetAccountIdAsync(string? token)
    {
        return token == null
            ? null
            : await _memoryCache.GetOrCreateAsync(token!, async cacheEntry =>
            {
                var storedToken = await _jwtStorageService.GetStoredTokenAsync(token);
                cacheEntry.AbsoluteExpiration = storedToken!.ValidTo;
                return storedToken.AccountId;
            });
    }

    public async Task<string> GenerateAsync(int accountId)
    {
        var storedToken = await _jwtStorageService.GenerateAsync(accountId);
        _memoryCache.Set(storedToken.Token!, storedToken.AccountId, storedToken.ValidTo);
        return storedToken.Token!;
    }
}

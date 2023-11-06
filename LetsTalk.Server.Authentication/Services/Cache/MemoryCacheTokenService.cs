using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Authentication.Models;
using Microsoft.Extensions.Caching.Memory;

namespace LetsTalk.Server.Authentication.Services.Cache;

public class MemoryCacheTokenService : IJwtStorageService
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

    public async Task<StoredToken?> GetStoredTokenAsync(string? token)
    {
        return token == null
            ? null
            : await _memoryCache.GetOrCreateAsync(token!, async cacheEntry =>
            {
                var storedToken = await _jwtStorageService.GetStoredTokenAsync(token);
                cacheEntry.AbsoluteExpiration = storedToken!.ValidTo;
                return storedToken;
            });
    }

    public async Task<StoredToken> GenerateJwtToken(int accountId)
    {
        var storedToken = await _jwtStorageService.GenerateJwtToken(accountId);
        _memoryCache.Set(storedToken.Token!, storedToken, storedToken.ValidTo);
        return storedToken;
    }
}

using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Core.Services.Cache.Profile;

public class ProfileMemoryCacheService(
    IMemoryCache memoryCache,
    IOptions<CachingSettings> cachingSettings,
    IProfileService profileService) : ProfileCacheServiceBase(profileService, cachingSettings), IProfileService, IProfileCacheManager
{
    private readonly IMemoryCache _memoryCache = memoryCache;

    public Task<ProfileDto> GetProfileAsync(string accountId, CancellationToken cancellationToken)
    {
        return IsActive
            ? _memoryCache.GetOrCreateAsync(GetProfileKey(accountId), cacheEntry =>
            {
                cacheEntry.SetAbsoluteExpiration(CacheLifeTimeInSeconds);
                return ProfileService.GetProfileAsync(accountId, cancellationToken);
            })!
            : ProfileService.GetProfileAsync(accountId, cancellationToken);
    }

    public Task ClearAsync(string accountId)
    {
        if (IsActive)
        {
            _memoryCache.Remove(GetProfileKey(accountId));
        }

        return Task.CompletedTask;
    }
}

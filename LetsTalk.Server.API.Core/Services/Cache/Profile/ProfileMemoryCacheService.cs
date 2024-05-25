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
        return _isActive
            ? _memoryCache.GetOrCreateAsync(GetProfileKey(accountId), cacheEntry =>
            {
                cacheEntry.SetAbsoluteExpiration(_cacheLifeTimeInSeconds);
                return _profileService.GetProfileAsync(accountId, cancellationToken);
            })!
            : _profileService.GetProfileAsync(accountId, cancellationToken);
    }

    public Task RemoveAsync(string accountId)
    {
        if (_isActive)
        {
            _memoryCache.Remove(GetProfileKey(accountId));
        }

        return Task.CompletedTask;
    }
}

using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Core.Services.Cache.Profile;

public class ProfileMemoryCacheService : ProfileCacheServiceBase, IProfileService, IProfileCacheManager
{
    private readonly IMemoryCache _memoryCache;

    public ProfileMemoryCacheService(
        IMemoryCache memoryCache,
        IOptions<CachingSettings> cachingSettings,
        IProfileService profileService) : base(profileService, cachingSettings)
    {
        _memoryCache = memoryCache;
    }

    public Task<AccountDto> GetProfileAsync(int accountId, CancellationToken cancellationToken)
    {
        return _isActive
            ? _memoryCache.GetOrCreateAsync(GetProfileKey(accountId), cacheEntry =>
            {
                cacheEntry.SetAbsoluteExpiration(_cacheLifeTimeInSeconds);
                return _profileService.GetProfileAsync(accountId, cancellationToken);
            })!
            : _profileService.GetProfileAsync(accountId, cancellationToken);
    }

    public Task RemoveAsync(int accountId)
    {
        if (_isActive)
        {
            _memoryCache.Remove(GetProfileKey(accountId));
        }

        return Task.CompletedTask;
    }
}

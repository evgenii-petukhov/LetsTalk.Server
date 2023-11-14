using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Core.Services.Cache.Profile;

public abstract class ProfileCacheServiceBase
{
    protected readonly bool _isActive;

    protected readonly bool _isVolotile;

    protected readonly TimeSpan _cacheLifeTimeInSeconds;

    protected readonly IProfileService _profileService;

    protected ProfileCacheServiceBase(
        IProfileService profileService,
        IOptions<CachingSettings> cachingSettings)
    {
        _profileService = profileService;

        _isActive = cachingSettings.Value.ProfileCacheLifeTimeInSeconds != 0;
        _isVolotile = _isActive && cachingSettings.Value.ProfileCacheLifeTimeInSeconds > 0;

        if (_isVolotile)
        {
            _cacheLifeTimeInSeconds = TimeSpan.FromSeconds(cachingSettings.Value.ProfileCacheLifeTimeInSeconds);
        }
    }

    protected static string GetProfileKey(int accountId)
    {
        return $"account:{accountId}:profile";
    }
}

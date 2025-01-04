﻿using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.API.Core.Abstractions;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Core.Services.Cache.Profile;

public abstract class ProfileCacheServiceBase
{
    protected bool IsActive { get; }

    protected bool IsVolotile { get; }

    protected TimeSpan CacheLifeTimeInSeconds { get; }

    protected IProfileService ProfileService { get; }

    protected ProfileCacheServiceBase(
        IProfileService profileService,
        IOptions<CachingSettings> cachingSettings)
    {
        ProfileService = profileService;

        IsActive = cachingSettings.Value.ProfileCacheLifeTimeInSeconds != 0;
        IsVolotile = IsActive && cachingSettings.Value.ProfileCacheLifeTimeInSeconds > 0;

        if (IsVolotile)
        {
            CacheLifeTimeInSeconds = TimeSpan.FromSeconds(cachingSettings.Value.ProfileCacheLifeTimeInSeconds);
        }
    }

    protected static string GetProfileKey(string accountId)
    {
        return $"profile:{accountId}";
    }
}

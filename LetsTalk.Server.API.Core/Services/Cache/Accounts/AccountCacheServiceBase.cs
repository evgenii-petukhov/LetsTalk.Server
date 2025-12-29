using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.API.Core.Abstractions;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Core.Services.Cache.Accounts;

public abstract class AccountCacheServiceBase
{
    protected const string AccountCacheKey = "accounts";

    protected bool IsActive { get; }

    protected bool IsVolatile { get; }

    protected TimeSpan CacheLifeTimeInSeconds { get; }

    protected IAccountService AccountService { get; }

    protected AccountCacheServiceBase(
        IAccountService accountService,
        IOptions<CachingSettings> cachingSettings)
    {
        AccountService = accountService;

        IsActive = cachingSettings.Value.AccountCacheLifeTimeInSeconds != 0;
        IsVolatile = IsActive && cachingSettings.Value.AccountCacheLifeTimeInSeconds > 0;

        if (IsVolatile)
        {
            CacheLifeTimeInSeconds = TimeSpan.FromSeconds(cachingSettings.Value.AccountCacheLifeTimeInSeconds);
        }
    }
}

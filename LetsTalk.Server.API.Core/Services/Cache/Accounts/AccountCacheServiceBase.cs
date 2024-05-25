using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Core.Services.Cache.Chats;

public abstract class AccountCacheServiceBase
{
    protected const string AccountCacheKey = "accounts";

    protected readonly bool _isActive;

    protected readonly bool _isVolotile;

    protected readonly TimeSpan _cacheLifeTimeInSeconds;

    protected readonly IAccountService _accountService;

    protected AccountCacheServiceBase(
        IAccountService accountService,
        IOptions<CachingSettings> cachingSettings)
    {
        _accountService = accountService;

        _isActive = cachingSettings.Value.AccountCacheLifeTimeInSeconds != 0;
        _isVolotile = _isActive && cachingSettings.Value.AccountCacheLifeTimeInSeconds > 0;

        if (_isVolotile)
        {
            _cacheLifeTimeInSeconds = TimeSpan.FromSeconds(cachingSettings.Value.AccountCacheLifeTimeInSeconds);
        }
    }
}

using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Core.Services.Cache.Contacts;

public abstract class ChatCacheServiceBase
{
    protected readonly bool _isActive;

    protected readonly bool _isVolotile;

    protected readonly TimeSpan _cacheLifeTimeInSeconds;

    protected readonly IChatService _accountService;

    protected ChatCacheServiceBase(
        IChatService accountService,
        IOptions<CachingSettings> cachingSettings)
    {
        _accountService = accountService;

        _isActive = cachingSettings.Value.ContactsCacheLifeTimeInSeconds != 0;
        _isVolotile = _isActive && cachingSettings.Value.ContactsCacheLifeTimeInSeconds > 0;

        if (_isVolotile)
        {
            _cacheLifeTimeInSeconds = TimeSpan.FromSeconds(cachingSettings.Value.ContactsCacheLifeTimeInSeconds);
        }
    }

    protected static string GetContactsKey(string accountId)
    {
        return $"account:{accountId}:contacts";
    }
}

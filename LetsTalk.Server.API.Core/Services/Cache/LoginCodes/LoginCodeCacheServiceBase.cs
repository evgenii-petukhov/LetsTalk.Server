using LetsTalk.Server.Configuration.Models;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Core.Services.Cache.Messages;

public abstract class LoginCodeCacheServiceBase
{
    protected readonly TimeSpan _cacheLifeTimeInSeconds;

    protected LoginCodeCacheServiceBase(
        IOptions<CachingSettings> cachingSettings)
    {
        _cacheLifeTimeInSeconds = TimeSpan.FromSeconds(cachingSettings.Value.LoginCodeCacheLifeTimeInSeconds);
    }

    protected static string GetLoginCodeKey(string email)
    {
        return $"login-code:{email}";
    }
}

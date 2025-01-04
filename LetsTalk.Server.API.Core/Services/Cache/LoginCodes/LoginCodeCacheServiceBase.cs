using LetsTalk.Server.Configuration.Models;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Core.Services.Cache.LoginCodes;

public abstract class LoginCodeCacheServiceBase(
    IOptions<CachingSettings> cachingSettings)
{
    protected TimeSpan CacheLifeTimeInSeconds { get; } = TimeSpan.FromSeconds(cachingSettings.Value.LoginCodeCacheLifeTimeInSeconds);

    protected static string GetLoginCodeKey(string email)
    {
        return $"login-code:{email}";
    }
}

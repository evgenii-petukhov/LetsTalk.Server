using LetsTalk.Server.Configuration.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using LetsTalk.Server.Authentication.Abstractions;

namespace LetsTalk.Server.Authentication.Services.Cache.LoginCodes;

public class LoginCodeMemoryCacheService(
    IMemoryCache memoryCache,
    ILoginCodeGenerator generator,
    IOptions<CachingSettings> cachingSettings) : LoginCodeCacheServiceBase(cachingSettings), ILoginCodeCacheService
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly ILoginCodeGenerator _generator = generator;

    public ValueTask<(int, bool, TimeSpan)> GenerateCodeAsync(string email)
    {
        var isCreated = false;

        var code = _memoryCache.GetOrCreate(GetLoginCodeKey(email), cacheEntry =>
        {
            cacheEntry.SetAbsoluteExpiration(CacheLifeTimeInSeconds);
            isCreated = true;
            return _generator.GenerateCode();
        });

        return new ValueTask<(int, bool, TimeSpan)>((code, isCreated, CacheLifeTimeInSeconds));
    }

    public ValueTask<bool> ValidateCodeAsync(string email, int code)
    {
        return new ValueTask<bool>(_memoryCache.Get<int>(GetLoginCodeKey(email)) == code);
    }
}

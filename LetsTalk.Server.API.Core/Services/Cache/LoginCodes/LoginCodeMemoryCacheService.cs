using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.API.Core.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using LetsTalk.Server.API.Core.Services.Cache.LoginCodes;

namespace LetsTalk.Server.API.Core.Services.Cache.Messages;

public class LoginCodeMemoryCacheService(
    IMemoryCache memoryCache,
    ILoginCodeGenerator generator,
    IOptions<CachingSettings> cachingSettings) : LoginCodeCacheServiceBase(cachingSettings), ILoginCodeCacheService
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly ILoginCodeGenerator _generator = generator;

    public Task<(int, bool, TimeSpan)> GenerateCodeAsync(string email)
    {
        var isCreated = false;

        var code = _memoryCache.GetOrCreate(GetLoginCodeKey(email), cacheEntry =>
        {
            cacheEntry.SetAbsoluteExpiration(_cacheLifeTimeInSeconds);
            isCreated = true;
            return _generator.GenerateCode();
        });

        return Task.FromResult((code, isCreated, _cacheLifeTimeInSeconds));
    }

    public Task<bool> ValidateCodeAsync(string email, int code)
    {
        return Task.FromResult(_memoryCache.Get<int>(GetLoginCodeKey(email)) == code);
    }
}

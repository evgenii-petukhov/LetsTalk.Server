using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Core.Services.Cache.Messages;

public class LoginCodeMemoryCacheService(
    IMemoryCache memoryCache,
    ILoginCodeGenerator generator,
    IOptions<CachingSettings> cachingSettings) : LoginCodeCacheServiceBase(cachingSettings), ILoginCodeCacheService
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly ILoginCodeGenerator _generator = generator;

    public Task<(int, bool)> GenerateCodeAsync(string email)
    {
        var isCreated = false;

        var code = _memoryCache.GetOrCreate(GetLoginCodeKey(email), cacheEntry =>
        {
            cacheEntry.SetAbsoluteExpiration(_cacheLifeTimeInSeconds);
            isCreated = true;
            return _generator.GenerateCode();
        });

        return Task.FromResult((code, isCreated));
    }

    public Task<bool> ValidateCodeAsync(string email, int code)
    {
        return Task.FromResult(_memoryCache.Get<int>(GetLoginCodeKey(email)) == code);
    }
}

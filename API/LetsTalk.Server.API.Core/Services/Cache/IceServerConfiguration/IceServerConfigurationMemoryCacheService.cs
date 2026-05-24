using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Configuration.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Core.Services.Cache.IceServerConfiguration;

public class IceServerConfigurationMemoryCacheService(
    IMemoryCache memoryCache,
    IIceServerConfigurationService iceServerConfigurationService,
    IOptions<CloudflareSettings> options) : IceServerConfigurationCacheServiceBase(
        iceServerConfigurationService,
        options), IIceServerConfigurationService
{
    private readonly IMemoryCache _memoryCache = memoryCache;

    public Task<string> GetIceServerConfigurationAsync(CancellationToken cancellationToken)
    {
        return _memoryCache.GetOrCreateAsync(Key, cacheEntry =>
        {
            cacheEntry.SetAbsoluteExpiration(CacheLifeTimeInSeconds);

            return IceServerConfigurationService.GetIceServerConfigurationAsync(cancellationToken);
        })!;
    }
}

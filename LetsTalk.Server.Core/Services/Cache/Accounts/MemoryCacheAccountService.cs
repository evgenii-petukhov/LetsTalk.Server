using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Core.Services.Cache.Messages;

public class MemoryCacheAccountService : CacheAccountServiceBase, IAccountService, IAccountCacheManager
{
    private readonly TimeSpan _cacheLifeTimeInSeconds;

    private readonly IMemoryCache _memoryCache;

    private readonly IAccountService _accountService;

    public MemoryCacheAccountService(
        IMemoryCache memoryCache,
        IOptions<CachingSettings> cachingSettings,
        IAccountService accountService)
    {
        _accountService = accountService;
        _memoryCache = memoryCache;
        _cacheLifeTimeInSeconds = TimeSpan.FromSeconds(cachingSettings.Value.ContactsCacheLifeTimeInSeconds);
    }

    public Task<List<AccountDto>> GetContactsAsync(int accountId, CancellationToken cancellationToken)
    {
        return _memoryCache.GetOrCreateAsync(GetContactsKey(accountId), cacheEntry =>
        {
            cacheEntry.SetAbsoluteExpiration(_cacheLifeTimeInSeconds);
            return _accountService.GetContactsAsync(accountId, cancellationToken);
        })!;
    }

    public Task<AccountDto> GetProfileAsync(int accountId, CancellationToken cancellationToken)
    {
        return _memoryCache.GetOrCreateAsync(GetProfileKey(accountId), cacheEntry =>
        {
            cacheEntry.SetAbsoluteExpiration(_cacheLifeTimeInSeconds);
            return _accountService.GetProfileAsync(accountId, cancellationToken);
        })!;
    }

    public Task RemoveAsync(int accountId)
    {
        _memoryCache.Remove(GetProfileKey(accountId));

        return Task.CompletedTask;
    }
}

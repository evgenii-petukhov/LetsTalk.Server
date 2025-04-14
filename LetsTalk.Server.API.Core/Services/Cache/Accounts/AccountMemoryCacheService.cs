using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Core.Services.Cache.Accounts;

public class AccountMemoryCacheService(
    IMemoryCache memoryCache,
    IOptions<CachingSettings> cachingSettings,
    IAccountService accountService) : AccountCacheServiceBase(accountService, cachingSettings), IAccountService, IAccountCacheManager
{
    private readonly IMemoryCache _memoryCache = memoryCache;

    public Task<IReadOnlyList<AccountDto>> GetAccountsAsync(CancellationToken cancellationToken)
    {
        return IsActive
            ? _memoryCache.GetOrCreateAsync(AccountCacheKey, cacheEntry =>
            {
                if (IsVolotile)
                {
                    cacheEntry.SetAbsoluteExpiration(CacheLifeTimeInSeconds);
                }

                return AccountService.GetAccountsAsync(cancellationToken);
            })!
            : AccountService.GetAccountsAsync(cancellationToken);
    }

    public Task ClearAsync()
    {
        if (IsActive)
        {
            _memoryCache.Remove(AccountCacheKey);
        }

        return Task.CompletedTask;
    }
}

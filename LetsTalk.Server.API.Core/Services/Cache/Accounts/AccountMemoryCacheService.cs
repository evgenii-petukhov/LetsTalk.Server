using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Core.Services.Cache.Chats;

public class AccountMemoryCacheService(
    IMemoryCache memoryCache,
    IOptions<CachingSettings> cachingSettings,
    IAccountService accountService) : AccountCacheServiceBase(accountService, cachingSettings), IAccountService
{
    private readonly IMemoryCache _memoryCache = memoryCache;

    public Task<IReadOnlyList<AccountDto>> GetAccountsAsync(CancellationToken cancellationToken)
    {
        return _isActive
            ? _memoryCache.GetOrCreateAsync(AccountCacheKey, cacheEntry =>
            {
                if (_isVolotile)
                {
                    cacheEntry.SetAbsoluteExpiration(_cacheLifeTimeInSeconds);
                }

                return _accountService.GetAccountsAsync(cancellationToken);
            })!
            : _accountService.GetAccountsAsync(cancellationToken);
    }
}

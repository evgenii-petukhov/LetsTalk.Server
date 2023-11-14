using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Core.Services.Cache.Contacts;

public class ContactsMemoryCacheService : ContactsCacheServiceBase, IContactsService
{
    private readonly IMemoryCache _memoryCache;

    public ContactsMemoryCacheService(
        IMemoryCache memoryCache,
        IOptions<CachingSettings> cachingSettings,
        IContactsService accountService) : base(accountService, cachingSettings)
    {
        _memoryCache = memoryCache;
    }

    public Task<List<AccountDto>> GetContactsAsync(int accountId, CancellationToken cancellationToken)
    {
        return _isActive
            ? _memoryCache.GetOrCreateAsync(GetContactsKey(accountId), cacheEntry =>
            {
                if (_isVolotile)
                {
                    cacheEntry.SetAbsoluteExpiration(_cacheLifeTimeInSeconds);
                }

                return _accountService.GetContactsAsync(accountId, cancellationToken);
            })!
            : _accountService.GetContactsAsync(accountId, cancellationToken);
    }
}

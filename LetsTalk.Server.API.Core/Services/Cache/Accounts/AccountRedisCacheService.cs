using System.Text.Json;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LetsTalk.Server.API.Core.Services.Cache.Chats;

public class AccountRedisCacheService(
    IConnectionMultiplexer сonnectionMultiplexer,
    IOptions<CachingSettings> cachingSettings,
    IAccountService accountService) : AccountCacheServiceBase(accountService, cachingSettings), IAccountService
{
    private readonly IDatabase _database = сonnectionMultiplexer.GetDatabase();

    public async Task<IReadOnlyList<AccountDto>> GetAccountsAsync(CancellationToken cancellationToken)
    {
        if (!_isActive)
        {
            return await _accountService.GetAccountsAsync(cancellationToken);
        }

        var key = new RedisKey(AccountCacheKey);

        var cachedAccounts = await _database.StringGetAsync(key);

        if (cachedAccounts == RedisValue.Null)
        {
            var accountDtos = await _accountService.GetAccountsAsync(cancellationToken);
            await _database.StringSetAsync(
                key,
                new RedisValue(JsonSerializer.Serialize(accountDtos)),
                when: When.NotExists);

            if (_isVolotile)
            {
                await _database.KeyExpireAsync(key, _cacheLifeTimeInSeconds);
            }

            return accountDtos;
        }

        return JsonSerializer.Deserialize<List<AccountDto>>(cachedAccounts!)!;
    }
}

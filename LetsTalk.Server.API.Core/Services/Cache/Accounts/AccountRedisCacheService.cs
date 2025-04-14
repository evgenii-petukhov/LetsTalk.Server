using System.Text.Json;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using Microsoft.Extensions.Options;
using LetsTalk.Server.Persistence.Redis;
using StackExchange.Redis;

namespace LetsTalk.Server.API.Core.Services.Cache.Accounts;

public class AccountRedisCacheService(
    RedisConnection redisConnection,
    IOptions<CachingSettings> cachingSettings,
    IAccountService accountService) : AccountCacheServiceBase(accountService, cachingSettings), IAccountService, IAccountCacheManager
{
    private readonly IDatabase _database = redisConnection.Connection.GetDatabase();

    public async Task<IReadOnlyList<AccountDto>> GetAccountsAsync(CancellationToken cancellationToken)
    {
        if (!IsActive)
        {
            return await AccountService.GetAccountsAsync(cancellationToken);
        }

        var key = new RedisKey(AccountCacheKey);

        var cachedAccounts = await _database.StringGetAsync(key);

        if (cachedAccounts == RedisValue.Null)
        {
            var accountDtos = await AccountService.GetAccountsAsync(cancellationToken);
            await _database.StringSetAsync(
                key,
                new RedisValue(JsonSerializer.Serialize(accountDtos)),
                when: When.NotExists);

            if (IsVolotile)
            {
                await _database.KeyExpireAsync(key, CacheLifeTimeInSeconds);
            }

            return accountDtos;
        }

        return JsonSerializer.Deserialize<List<AccountDto>>(cachedAccounts!)!;
    }

    public async Task ClearAsync()
    {
        if (IsActive)
        {
            await _database.KeyDeleteAsync(AccountCacheKey, CommandFlags.FireAndForget);
        }
    }
}

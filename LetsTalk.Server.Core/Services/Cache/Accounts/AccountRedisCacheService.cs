using System.Text.Json;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LetsTalk.Server.Core.Services.Cache.Chats;

public class AccountRedisCacheService(
    IConnectionMultiplexer сonnectionMultiplexer,
    IOptions<CachingSettings> cachingSettings,
    IAccountService accountService) : AccountCacheServiceBase(accountService, cachingSettings), IAccountService
{
    private readonly IDatabase _database = сonnectionMultiplexer.GetDatabase();

    public async Task<List<AccountDto>> GetAccountsAsync(string id, CancellationToken cancellationToken)
    {
        if (!_isActive)
        {
            return await _accountService.GetAccountsAsync(id, cancellationToken);
        }

        var key = new RedisKey(GetAccountsKey(id));

        var cachedAccounts = await _database.StringGetAsync(key);

        if (cachedAccounts == RedisValue.Null)
        {
            var accountDtos = await _accountService.GetAccountsAsync(id, cancellationToken);
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

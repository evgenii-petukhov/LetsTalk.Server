﻿using System.Text.Json;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace LetsTalk.Server.Core.Services.Cache.Messages;

public class RedisCacheAccountService : CacheMessageServiceBase, IAccountService
{
    private readonly TimeSpan _cacheLifeTimeInSeconds;

    private readonly IDatabase _database;

    private readonly IAccountService _accountService;

    public RedisCacheAccountService(
        IConnectionMultiplexer сonnectionMultiplexer,
        IOptions<CachingSettings> cachingSettings,
        IAccountService accountService)
    {
        _accountService = accountService;
        _database = сonnectionMultiplexer.GetDatabase();
        _cacheLifeTimeInSeconds = TimeSpan.FromSeconds(cachingSettings.Value.ContactsCacheLifeTimeInSeconds);
    }

    public async Task<List<AccountDto>> GetContactsAsync(int accountId, CancellationToken cancellationToken)
    {
        var key = new RedisKey($"Accounts:{accountId}");

        var cachedAccounts = await _database.StringGetAsync(key);

        if (cachedAccounts == RedisValue.Null)
        {
            var accountDtos = await _accountService.GetContactsAsync(accountId, cancellationToken);
            await _database.StringSetAsync(
                key,
                new RedisValue(JsonSerializer.Serialize(accountDtos)),
                _cacheLifeTimeInSeconds,
                When.NotExists);

            await _database.KeyExpireAsync(key, _cacheLifeTimeInSeconds);

            return accountDtos;
        }

        return JsonSerializer.Deserialize<List<AccountDto>>(cachedAccounts!)!;
    }
}
using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LetsTalk.Server.Persistence.Services;

public class AccountDataLayerService : GenericDataLayerService, IAccountDataLayerService
{
    private readonly IAccountRepository _accountRepository;

    public AccountDataLayerService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<int> CreateOrUpdateAsync(
        string externalId,
        AccountTypes accountType,
        string? firstName,
        string? lastName,
        string? email,
        string? photoUrl,
        CancellationToken cancellationToken = default)
    {
        var response = await GetByExternalIdAsync(externalId, accountType, x => new
        {
            x.Id,
            x.ImageId
        }, cancellationToken);

        if (response.HasValue)
        {
            var account = response.Value!;
            await (account.ImageId.HasValue
                ? _accountRepository.UpdateAsync(account.Id, firstName, lastName, email, cancellationToken)
                : _accountRepository.UpdateAsync(account.Id, firstName, lastName, email, photoUrl, cancellationToken));

            return account.Id;
        }
        else
        {
            try
            {
                var account = new Account
                {
                    ExternalId = externalId,
                    AccountTypeId = (int)accountType,
                    FirstName = firstName,
                    LastName = lastName,
                    PhotoUrl = photoUrl,
                    Email = email
                };
                await _accountRepository.CreateAsync(account, cancellationToken);
                return account.Id;
            }
            catch (DbUpdateException)
            {
                return (await GetByExternalIdAsync(externalId, accountType, x => x.Id, cancellationToken)).Value;
            }
        }
    }

    public async Task<T?> GetByIdAsync<T>(int id, Expression<Func<Account, T>> selector, bool includeFile = false, CancellationToken cancellationToken = default)
    {
        var query = _accountRepository.GetById(id, includeFile)
            .Select(selector);
        var response = await GetSingleValueAsync(query, cancellationToken);
        return response.HasValue ? response.Value : default;
    }

    public async Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default)
    {
        var query = _accountRepository.GetById(id)
            .Select(x => x.Id);
        var response = await GetSingleValueAsync(query, cancellationToken);
        return response.HasValue;
    }

    private Task<QuerySingleResponse<T>> GetByExternalIdAsync<T>(string externalId, AccountTypes accountType, Expression<Func<Account, T>> selector, CancellationToken cancellationToken = default)
    {
        var query = _accountRepository.GetByExternalId(externalId, accountType)
            .Select(selector);
        return GetSingleValueAsync(query, cancellationToken);
    }
}

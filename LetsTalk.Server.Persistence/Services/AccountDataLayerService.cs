using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LetsTalk.Server.Persistence.Services;

public class AccountDataLayerService : IAccountDataLayerService
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
        var response = await GetByExternalIdOrDefaultAsync(externalId, accountType, x => new
        {
            x.Id,
            x.ImageId
        }, cancellationToken);

        if (response == null)
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
                return await GetByExternalIdOrDefaultAsync(externalId, accountType, x => x.Id, cancellationToken);
            }
        }
        else
        {
            await (response.ImageId.HasValue
                ? _accountRepository.UpdateAsync(response.Id, firstName, lastName, email, cancellationToken)
                : _accountRepository.UpdateAsync(response.Id, firstName, lastName, email, photoUrl, cancellationToken));

            return response.Id;
        }
    }

    public Task<T?> GetByIdOrDefaultAsync<T>(int id, Expression<Func<Account, T>> selector, bool includeFile = false, CancellationToken cancellationToken = default)
    {
        return _accountRepository.GetById(id, includeFile)
            .Select(selector)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default)
    {
        return _accountRepository.GetById(id)
            .Select(x => x.Id)
            .AnyAsync(cancellationToken: cancellationToken);
    }

    private Task<T?> GetByExternalIdOrDefaultAsync<T>(string externalId, AccountTypes accountType, Expression<Func<Account, T>> selector, CancellationToken cancellationToken = default)
    {
        return _accountRepository.GetByExternalId(externalId, accountType)
            .Select(selector)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }
}

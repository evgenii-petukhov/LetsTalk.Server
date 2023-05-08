using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.Services;

public class AccountDataLayerService : IAccountDataLayerService
{
    private readonly IAccountRepository _accountRepository;

    public AccountDataLayerService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<int> CreateOrUpdate(
        string externalId,
        AccountTypes accountType,
        string? firstName,
        string? lastName,
        string? photoUrl,
        string? email = null)
    {
        var account = await _accountRepository.GetByExternalIdAsync(externalId, accountType);
        if (account == null)
        {
            try
            {
                account = new Account
                {
                    ExternalId = externalId,
                    AccountTypeId = (int)accountType,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PhotoUrl = photoUrl
                };
                await _accountRepository.CreateAsync(account);
                return account.Id;
            }
            catch (DbUpdateException)
            {
                account = await _accountRepository.GetByExternalIdAsync(externalId, accountType);
            }
        }

        await _accountRepository.UpdateAsync(account!.Id, firstName, lastName, email, photoUrl);
        return account.Id;
    }
}

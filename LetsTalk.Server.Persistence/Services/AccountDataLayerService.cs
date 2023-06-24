using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.EntityFrameworkCore;

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
        CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByExternalIdAsync(externalId, accountType, cancellationToken);
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
                    PhotoUrl = photoUrl,
                    Email = email
                };
                await _accountRepository.CreateAsync(account, cancellationToken);
                return account.Id;
            }
            catch (DbUpdateException)
            {
                account = await _accountRepository.GetByExternalIdAsync(externalId, accountType, cancellationToken);
            }
        }

        if (account!.ImageId.HasValue)
        {
            await _accountRepository.UpdateAsync(account!.Id, firstName, lastName, email, cancellationToken);
        }
        else
        {
            await _accountRepository.UpdateAsync(account!.Id, firstName, lastName, email, photoUrl, cancellationToken);
        }

        return account.Id;
    }
}

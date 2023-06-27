using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

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
        try
        {
            var account = await _accountRepository.GetByExternalId(externalId, accountType)
                .FirstAsync(cancellationToken);

            await (account.ImageId.HasValue
                ? _accountRepository.UpdateAsync(account.Id, firstName, lastName, email, cancellationToken)
                : _accountRepository.UpdateAsync(account.Id, firstName, lastName, email, photoUrl, cancellationToken));

            return account.Id;
        }
        catch (InvalidOperationException)
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
                return await _accountRepository.GetByExternalId(externalId, accountType)
                    .Select(x => x.Id)
                    .FirstAsync(cancellationToken);
            }
        }
    }
}

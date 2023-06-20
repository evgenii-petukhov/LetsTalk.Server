using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.Services;

public class AccountDataLayerService : IAccountDataLayerService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IImageRepository _imageRepository;

    public AccountDataLayerService(
        IAccountRepository accountRepository,
        IImageRepository imageRepository)
    {
        _accountRepository = accountRepository;
        _imageRepository = imageRepository;
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

    public async Task UpdateAsync(int id, string? firstName, string? lastName, string? email, int? imageId, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(id, cancellationToken);
        if (account == null) return;

        await _accountRepository.UpdateAsync(id, firstName, lastName, email, null, imageId, cancellationToken);
        if (account.ImageId.HasValue)
        {
            var image = await _imageRepository.GetByIdAsync(account.ImageId.Value, cancellationToken);
            if (image != null)
            {
                await _imageRepository.DeleteAsync(image.Id);
            }
        }
    }
}

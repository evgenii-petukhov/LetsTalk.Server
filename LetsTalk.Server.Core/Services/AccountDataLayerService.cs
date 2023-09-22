using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Core.Services;

public class AccountDataLayerService: IAccountDataLayerService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IEntityFactory _entityFactory;
    private readonly IUnitOfWork _unitOfWork;

    public AccountDataLayerService(
        IAccountRepository accountRepository,
        IEntityFactory entityFactory,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _entityFactory = entityFactory;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> CreateOrUpdateAsync(string externalId, AccountTypes accountType, string? firstName, string? lastName,
    string? email, string? photoUrl, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByExternalIdAsTrackingAsync(externalId, accountType, cancellationToken);

        if (account == null)
        {
            try
            {
                account = _entityFactory.CreateAccount(externalId, (int)accountType, firstName!, lastName!, photoUrl!, email!);
                await _accountRepository.CreateAsync(account, cancellationToken);
                await _unitOfWork.SaveAsync(cancellationToken);
                return account.Id;
            }
            catch (DbUpdateException)
            {
                return (await _accountRepository.GetByExternalIdAsync(externalId, accountType, cancellationToken)).Id;
            }
        }
        else
        {
            account.SetupProfile(firstName!, lastName!, email!, photoUrl!, account.ImageId.HasValue);

            await _unitOfWork.SaveAsync(cancellationToken);

            return account.Id;
        }
    }
}

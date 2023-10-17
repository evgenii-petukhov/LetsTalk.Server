using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Core.Services;

public class AccountDataLayerService: IAccountDataLayerService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAccountDomainService _accountDomainService;
    private readonly IUnitOfWork _unitOfWork;

    public AccountDataLayerService(
        IAccountRepository accountRepository,
        IAccountDomainService accountDomainService,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _accountDomainService = accountDomainService;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> CreateOrUpdateAsync(
        string externalId,
        AccountTypes accountType,
        string firstName,
        string lastName,
        string email,
        string photoUrl,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByExternalIdAsTrackingAsync(externalId, accountType, cancellationToken);

        if (account == null)
        {
            try
            {
                account = await _accountDomainService.CreateAccountAsync(externalId, accountType, firstName, lastName, email, photoUrl, cancellationToken);
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

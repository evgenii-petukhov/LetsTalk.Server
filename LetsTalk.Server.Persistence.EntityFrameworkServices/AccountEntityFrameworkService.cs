using AutoMapper;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.EntityFrameworkServices;

public class AccountEntityFrameworkService: IAccountAgnosticService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IEntityFactory _entityFactory;

    public AccountEntityFrameworkService(
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IEntityFactory entityFactory)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _entityFactory = entityFactory;
    }

    public Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default)
    {
        return _accountRepository.IsAccountIdValidAsync(id, cancellationToken);
    }

    public async Task<AccountServiceModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(id, cancellationToken);

        return _mapper.Map<AccountServiceModel>(account);
    }

    public async Task<AccountServiceModel> UpdateProfileAsync(
        int accountId,
        string firstName,
        string lastName,
        string email,
        int? imageId,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsTrackingAsync(accountId, cancellationToken);
        account.UpdateProfile(firstName, lastName, email, imageId);

        await _unitOfWork.SaveAsync(cancellationToken);

        return _mapper.Map<AccountServiceModel>(account);
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
                account = await CreateAccountAsync(externalId, accountType, firstName, lastName, email, photoUrl, cancellationToken);
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

    public async Task<List<ContactServiceModel>> GetContactsAsync(int id, CancellationToken cancellationToken = default)
    {
        var contacts = await _accountRepository.GetContactsAsync(id, cancellationToken);

        return _mapper.Map<List<ContactServiceModel>>(contacts);
    }

    private async Task<Account> CreateAccountAsync(
        string externalId,
        AccountTypes accountType,
        string firstName,
        string lastName,
        string email,
        string photoUrl,
        CancellationToken cancellationToken = default)
    {
        var account = _entityFactory.CreateAccount(externalId, (int)accountType, firstName!, lastName!, email!, photoUrl!);
        await _accountRepository.CreateAsync(account, cancellationToken);
        return account;
    }
}

using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.EntityFramework.Services;

public class AccountEntityFrameworkService: IAccountAgnosticService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IImageRepository _imageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IEntityFactory _entityFactory;

    public AccountEntityFrameworkService(
        IAccountRepository accountRepository,
        IImageRepository imageRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IEntityFactory entityFactory)
    {
        _accountRepository = accountRepository;
        _imageRepository = imageRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _entityFactory = entityFactory;
    }

    public Task<bool> IsAccountIdValidAsync(string id, CancellationToken cancellationToken = default)
    {
        return _accountRepository.IsAccountIdValidAsync(int.Parse(id), cancellationToken);
    }

    public async Task<AccountServiceModel> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(int.Parse(id), cancellationToken);

        return _mapper.Map<AccountServiceModel>(account);
    }

    public async Task<AccountServiceModel> UpdateProfileAsync(
        string accountId,
        string firstName,
        string lastName,
        string email,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsTrackingAsync(int.Parse(accountId), cancellationToken);
        account.UpdateProfile(firstName, lastName, email);

        await _unitOfWork.SaveAsync(cancellationToken);

        return _mapper.Map<AccountServiceModel>(account);
    }

    public async Task<AccountServiceModel> UpdateProfileAsync(
        string accountId,
        string firstName,
        string lastName,
        string email,
        string imageId,
        int width,
        int height,
        ImageFormats imageFormat,
        CancellationToken cancellationToken = default)
    {
        var image = _entityFactory.CreateImage(imageId, imageFormat, width, height);
        var account = await _accountRepository.GetByIdAsTrackingAsync(int.Parse(accountId), cancellationToken);

        if (account.Image != null && !string.IsNullOrEmpty(imageId))
        {
            _imageRepository.Delete(account.Image);
        }

        account.UpdateProfile(firstName, lastName, email, image);

        await _unitOfWork.SaveAsync(cancellationToken);

        return _mapper.Map<AccountServiceModel>(account);
    }

    public async Task<string> CreateOrUpdateAsync(
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
                account = _entityFactory.CreateAccount(externalId, (int)accountType, firstName!, lastName!, email!, photoUrl!);
                await _accountRepository.CreateAsync(account, cancellationToken);
                await _unitOfWork.SaveAsync(cancellationToken);

                return account.Id.ToString();
            }
            catch (DbUpdateException)
            {
                return (await _accountRepository.GetByExternalIdAsync(externalId, accountType, cancellationToken)).Id.ToString();
            }
        }
        else
        {
            account.SetupProfile(firstName!, lastName!, email!, photoUrl!, !string.IsNullOrEmpty(account.ImageId));
            await _unitOfWork.SaveAsync(cancellationToken);

            return account.Id.ToString();
        }
    }

    public async Task<List<ContactServiceModel>> GetContactsAsync(string id, CancellationToken cancellationToken = default)
    {
        var contacts = await _accountRepository.GetContactsAsync(int.Parse(id), cancellationToken);

        return _mapper.Map<List<ContactServiceModel>>(contacts);
    }
}

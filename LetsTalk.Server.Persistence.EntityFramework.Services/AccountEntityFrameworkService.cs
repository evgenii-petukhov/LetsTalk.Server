using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace LetsTalk.Server.Persistence.EntityFramework.Services;

public class AccountEntityFrameworkService(
    IAccountRepository accountRepository,
    IImageRepository imageRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IEntityFactory entityFactory) : IAccountAgnosticService
{
    private readonly IAccountRepository _accountRepository = accountRepository;
    private readonly IImageRepository _imageRepository = imageRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly IEntityFactory _entityFactory = entityFactory;

    public async Task<ProfileServiceModel> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(int.Parse(id, CultureInfo.InvariantCulture), cancellationToken);

        return _mapper.Map<ProfileServiceModel>(account);
    }

    public async Task<ProfileServiceModel> UpdateProfileAsync(
        string accountId,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsTrackingAsync(int.Parse(accountId, CultureInfo.InvariantCulture), cancellationToken);
        account.UpdateProfile(firstName, lastName);

        await _unitOfWork.SaveAsync(cancellationToken);

        return _mapper.Map<ProfileServiceModel>(account);
    }

    public async Task<ProfileServiceModel> UpdateProfileAsync(
        string accountId,
        string firstName,
        string lastName,
        string imageId,
        int width,
        int height,
        ImageFormats imageFormat,
        CancellationToken cancellationToken = default)
    {
        var image = _entityFactory.CreateImage(imageId, imageFormat, width, height);
        var account = await _accountRepository.GetByIdAsTrackingAsync(int.Parse(accountId, CultureInfo.InvariantCulture), cancellationToken);

        if (account.Image != null && !string.IsNullOrEmpty(imageId))
        {
            _imageRepository.Delete(account.Image);
        }

        account.UpdateProfile(firstName, lastName, image);

        await _unitOfWork.SaveAsync(cancellationToken);

        return _mapper.Map<ProfileServiceModel>(account);
    }

    public async Task<string> CreateOrUpdateAsync(
        string externalId,
        AccountTypes accountType,
        string firstName,
        string lastName,
        string photoUrl,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByExternalIdAsTrackingAsync(externalId, accountType, cancellationToken);

        if (account == null)
        {
            try
            {
                account = _entityFactory.CreateAccount(externalId, (int)accountType, firstName!, lastName!, photoUrl!);
                await _accountRepository.CreateAsync(account, cancellationToken);
                await _unitOfWork.SaveAsync(cancellationToken);

                return account.Id.ToString(CultureInfo.InvariantCulture);
            }
            catch (DbUpdateException)
            {
                return (await _accountRepository.GetByExternalIdAsync(externalId, accountType, cancellationToken)).Id.ToString(CultureInfo.InvariantCulture);
            }
        }
        else
        {
            account.SetupProfile(firstName!, lastName!, photoUrl!, !string.IsNullOrEmpty(account.ImageId));
            await _unitOfWork.SaveAsync(cancellationToken);

            return account.Id.ToString(CultureInfo.InvariantCulture);
        }
    }

    public async Task<string> GetOrCreateAsync(AccountTypes accountType, string email, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByEmailAsync(email, accountType, cancellationToken);

        if (account != null)
        {
            return account.Id.ToString(CultureInfo.InvariantCulture);
        }

        try
        {
            account = _entityFactory.CreateAccount((int)accountType, email!);
            await _accountRepository.CreateAsync(account, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            return account.Id.ToString(CultureInfo.InvariantCulture);
        }
        catch (DbUpdateException)
        {
            return (await _accountRepository.GetByEmailAsync(email, accountType, cancellationToken)).Id.ToString(CultureInfo.InvariantCulture);
        }
    }

    public async Task<List<AccountServiceModel>> GetAccountsAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await _accountRepository.GetAccountsAsync(cancellationToken);

        return _mapper.Map<List<AccountServiceModel>>(accounts);
    }

    public Task<bool> IsAccountIdValidAsync(string id, CancellationToken cancellationToken = default)
    {
        return _accountRepository.IsAccountIdValidAsync(int.Parse(id, CultureInfo.InvariantCulture), cancellationToken);
    }
}

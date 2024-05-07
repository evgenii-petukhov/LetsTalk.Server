using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.MongoDB.Services;

public class AccountMongoDBService(
    IAccountRepository accountRepository,
    IMapper mapper) : IAccountAgnosticService
{
    private readonly IAccountRepository _accountRepository = accountRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<ProfileServiceModel> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(id, cancellationToken);

        return _mapper.Map<ProfileServiceModel>(account);
    }

    public async Task<ProfileServiceModel> UpdateProfileAsync(
        string accountId,
        string firstName,
        string lastName,
        string email,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.UpdateProfileAsync(accountId, firstName, lastName, email, cancellationToken);

        return _mapper.Map<ProfileServiceModel>(account);
    }

    public async Task<ProfileServiceModel> UpdateProfileAsync(
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
        var account = await _accountRepository.UpdateProfileAsync(
            accountId,
            firstName,
            lastName,
            email,
            imageId,
            width,
            height,
            imageFormat,
            cancellationToken);

        return _mapper.Map<ProfileServiceModel>(account);
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
        var acccount = await _accountRepository.GetByExternalIdAsync(externalId, accountType, cancellationToken);

        if (acccount == null)
        {
            try
            {
                acccount = await _accountRepository.CreateAccountAsync(
                    externalId,
                    accountType,
                    firstName,
                    lastName,
                    email,
                    photoUrl,
                    cancellationToken);

                return acccount.Id!;
            }
            catch
            {
                return (await _accountRepository.GetByExternalIdAsync(externalId, accountType, cancellationToken)).Id!;
            }
        }

        var account = await _accountRepository.SetupProfileAsync(
            externalId,
            accountType,
            firstName,
            lastName,
            email,
            photoUrl,
            acccount.Image == null,
            cancellationToken);

        return account.Id!;
    }

    public async Task<List<AccountServiceModel>> GetAccountsAsync(string id, CancellationToken cancellationToken = default)
    {
        var accounts = await _accountRepository.GetAccountsAsync(id, cancellationToken);

        return _mapper.Map<List<AccountServiceModel>>(accounts);
    }

    public Task<bool> IsAccountIdValidAsync(string id, CancellationToken cancellationToken = default)
    {
        return _accountRepository.IsAccountIdValidAsync(id, cancellationToken);
    }
}

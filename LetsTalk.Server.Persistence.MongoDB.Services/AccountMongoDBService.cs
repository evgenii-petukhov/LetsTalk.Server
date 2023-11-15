using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.MongoDB.Services;

public class AccountMongoDBService : IAccountAgnosticService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public AccountMongoDBService(
        IAccountRepository accountRepository,
        IMapper mapper)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    public Task<bool> IsAccountIdValidAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<AccountServiceModel> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(id, cancellationToken);

        return _mapper.Map<AccountServiceModel>(account);
    }

    public async Task<AccountServiceModel> UpdateProfileAsync(
        string accountId,
        string firstName,
        string lastName,
        string email,
        int? imageId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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
        var account = await _accountRepository.SetupProfile(
            externalId,
            (int)accountType,
            firstName,
            lastName,
            email,
            photoUrl,
            cancellationToken);

        if (account == null)
        {
            try
            {
                account = await _accountRepository.CreateAccountAsync(
                    externalId,
                    (int)accountType,
                    firstName,
                    lastName,
                    email,
                    photoUrl,
                    cancellationToken);

                return account.Id!;
            }
            catch
            {
                return (await _accountRepository.GetByExternalIdAsync(externalId, (int)accountType, cancellationToken)).Id!;
            }
        }

        return account.Id!;
    }

    public async Task<List<ContactServiceModel>> GetContactsAsync(string id, CancellationToken cancellationToken = default)
    {
        var contacts = await _accountRepository.GetContactsAsync(id, cancellationToken);

        return _mapper.Map<List<ContactServiceModel>>(contacts);
    }
}

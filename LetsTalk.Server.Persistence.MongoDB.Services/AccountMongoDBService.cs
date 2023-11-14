using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.MongoDB.Services;

public class AccountMongoDBService : IAccountAgnosticService
{
    private readonly IAccountRepository _accountRepository;

    public AccountMongoDBService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public Task<bool> IsAccountIdValidAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<AccountServiceModel> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
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
        var account = await _accountRepository.GetByExternalIdAsync(externalId, (int)accountType, cancellationToken);

        if (account == null)
        {
            try
            {
                account = await _accountRepository.CreateAccountAsync(externalId, (int)accountType, firstName, lastName, email, photoUrl);
                return account.Id;
            }
            catch
            {
                //return (await _accountRepository.GetByExternalIdAsync(externalId, accountType, cancellationToken)).Id;
            }
        }
        else
        {
            /*account.SetupProfile(firstName!, lastName!, email!, photoUrl!, account.ImageId.HasValue);

            await _unitOfWork.SaveAsync(cancellationToken);

            return account.Id;*/
        }

        return "";
    }

    public async Task<List<ContactServiceModel>> GetContactsAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

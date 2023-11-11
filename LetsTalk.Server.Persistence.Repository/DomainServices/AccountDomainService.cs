using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.Repository.DomainServices;

public class AccountDomainService : IAccountDomainService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IEntityFactory _entityFactory;

    public AccountDomainService(
        IAccountRepository accountRepository,
        IEntityFactory entityFactory)
    {
        _accountRepository = accountRepository;
        _entityFactory = entityFactory;
    }

    public async Task<Account> CreateAccountAsync(
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

    public async Task<Account> UpdateProfileAsync(
        int accountId,
        string firstName,
        string lastName,
        string email,
        int? imageId,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsTrackingAsync(accountId, cancellationToken);
        account.UpdateProfile(firstName, lastName, email, imageId);
        return account;
    }
}

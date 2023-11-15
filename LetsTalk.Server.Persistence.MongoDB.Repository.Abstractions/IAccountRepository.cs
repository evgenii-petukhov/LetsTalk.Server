using LetsTalk.Server.Persistence.MongoDB.Models;

namespace LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;

public interface IAccountRepository
{
    Task<Account> GetByExternalIdAsync(
        string externalId,
        int accountTypeId,
        CancellationToken cancellationToken = default);

    Task<Account> CreateAccountAsync(
        string externalId,
        int accountTypeId,
        string firstName,
        string lastName,
        string email,
        string photoUrl,
        CancellationToken cancellationToken);

    Task<Account> SetupProfile(
        string externalId,
        int accountTypeId,
        string firstName,
        string lastName,
        string email,
        string photoUrl,
        bool updateAvatar,
        CancellationToken cancellationToken);

    Task<List<Contact>> GetContactsAsync(string id, CancellationToken cancellationToken = default);

    Task<bool> IsAccountIdValidAsync(string id, CancellationToken cancellationToken = default);

    Task<Account> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}

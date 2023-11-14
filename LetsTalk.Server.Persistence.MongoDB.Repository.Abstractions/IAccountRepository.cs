using LetsTalk.Server.Persistence.MongoDB.Models;

namespace LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;

public interface IAccountRepository
{
    Task<Account> GetByExternalIdAsync(string externalId, int accountTypeId, CancellationToken cancellationToken);

    Task<Account> CreateAccountAsync(string externalId, int accountTypeId, string firstName, string lastName, string email, string photoUrl);
}

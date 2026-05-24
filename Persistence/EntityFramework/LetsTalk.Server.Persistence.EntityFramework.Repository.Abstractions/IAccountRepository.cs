using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

public interface IAccountRepository : IGenericRepository<Account>
{
    Task<List<Account>> GetAccountsAsync(CancellationToken cancellationToken = default);

    Task<Account> GetByEmailAsync(string email, AccountTypes accountType, CancellationToken cancellationToken = default);

    Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default);
}

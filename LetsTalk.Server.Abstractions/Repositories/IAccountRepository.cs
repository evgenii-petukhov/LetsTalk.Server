using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Abstractions.Repositories
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<Account?> GetByExternalIdAsync(string externalId);

        Task<IReadOnlyList<AccountWithUnreadCount>> GetOtherAsync(int id);

        Task<Account?> GetByIdAsync(int id);
    }
}

using LetsTalk.Server.Abstractions.Repositories;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.Repositories
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(LetsTalkDbContext context) : base(context)
        {

        }

        public async Task<Account> GetByExternalIdAsync(string externalId)
        {
            return await _context.Set<Account>()
                .AsNoTracking()
                .SingleOrDefaultAsync(q => q.ExternalId == externalId);
        }

        public async Task<IReadOnlyList<Account>> GetOtherAsync(int id)
        {
            return await _context.Set<Account>()
                .Where(account => account.Id != id)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}

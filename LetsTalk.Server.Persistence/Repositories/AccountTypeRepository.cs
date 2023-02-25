using LetsTalk.Server.Abstractions.Repositories;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;

namespace LetsTalk.Server.Persistence.Repositories
{
    public class AccountTypeRepository : GenericRepository<AccountType>, IAccountTypeRepository
    {
        public AccountTypeRepository(LetsTalkDbContext context) : base(context)
        {
        }
    }
}

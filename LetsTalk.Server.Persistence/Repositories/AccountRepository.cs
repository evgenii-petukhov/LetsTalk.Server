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

        public async Task<IReadOnlyList<AccountWithUnreadCount>> GetOtherAsync(int id)
        {
            var accountsWithUnread = _context.Set<Account>()
                .Where(account => account.Id != id)
                .Join(
                    _context.Set<Message>().Where(message => message.RecipientId == id && message.DateCreated <= DateTime.Now && message.IsRead == false),
                    account => account.Id,
                    message => message.SenderId,
                    (account, message) => new
                    {
                        Account = account,
                        Message = message
                    })
                .GroupBy(g => g.Account)
                .Where(g => g.Any())
                .Select(g => new
                {
                    AccountId = g.Key.Id,
                    UnreadCount = g.Count(),
                    LastMessageDate = g.Max(x => x.Message.DateCreated)
                });

            return await _context.Set<Account>().Where(account => account.Id != id)
                .GroupJoin(accountsWithUnread, x => x.Id, x => x.AccountId, (x, y) => new
                {
                    Account = x,
                    AccountWithUnreads = y
                })
                .SelectMany(
                    x => x.AccountWithUnreads.DefaultIfEmpty(),
                    (x, y) => new
                    {
                        x.Account,
                        AccountWithUnread = y
                    })
                .Select(x => new AccountWithUnreadCount
                {
                    Id = x.Account.Id,
                    FirstName = x.Account.FirstName,
                    LastName = x.Account.LastName,
                    PhotoUrl = x.Account.PhotoUrl,
                    AccountTypeId = x.Account.AccountTypeId,
                    UnreadCount = x.AccountWithUnread.UnreadCount,
                    LastMessageDate = x.AccountWithUnread.LastMessageDate
                })
                .ToListAsync();
        }

        public async Task<Account> GetByIdAsync(int id)
        {
            return await _context.Set<Account>()
                .SingleOrDefaultAsync(account => account.Id == id);
        }
    }
}

class A
{
    public int AccountId {get; set;}
    public int UnreadCount { get; set; }
}

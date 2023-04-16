using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.Repositories
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(LetsTalkDbContext context) : base(context)
        {
        }

        public Task<Account?> GetByExternalIdAsync(string externalId)
        {
            return _context.Accounts
                .AsNoTracking()
                .SingleOrDefaultAsync(q => q.ExternalId == externalId);
        }

        public async Task<IReadOnlyList<AccountWithUnreadCount>> GetOtherAsync(int id)
        {
            var sentMessageDates = _context.Messages
                .AsNoTracking()
                .Where(x => x.SenderId == id)
                .GroupBy(x => x.RecipientId)
                .Select(g => new
                {
                    AccountId = g.Key,
                    LastMessageDate = g.Max(x => x.DateCreatedUnix)
                });

            var receivedMessageDates = _context.Messages
                .AsNoTracking()
                .Where(x => x.RecipientId == id)
                .GroupBy(x => x.SenderId)
                .Select(g => new
                {
                    AccountId = g.Key,
                    LastMessageDate = g.Max(x => x.DateCreatedUnix)
                });

            var lastMessageDates = sentMessageDates
                .Concat(receivedMessageDates)
                .GroupBy(x => x.AccountId)
                .Select(x => new
                {
                    AccountId = x.Key,
                    LastMessageDate = x.Max(a => a.LastMessageDate)
                });

            var unreadMessageCounts = _context.Set<Account>()
                .Where(account => account.Id != id)
                .Join(
                    _context.Messages
                        .AsNoTracking()
                        .Where(message => message.RecipientId == id && !message.IsRead),
                    account => account.Id,
                    message => message.SenderId,
                    (account, message) => new
                    {
                        Account = account,
                        Message = message
                    })
                .GroupBy(g => g.Account)
                .Select(g => new
                {
                    AccountId = g.Key.Id,
                    UnreadCount = g.Count()
                });

            return await _context.Accounts
                .AsNoTracking()
                .Where(account => account.Id != id)
                .GroupJoin(lastMessageDates, x => x.Id, x => x.AccountId, (x, y) => new
                {
                    Account = x,
                    LastMessageDates = y
                })
                .SelectMany(
                    x => x.LastMessageDates.DefaultIfEmpty(),
                    (x, y) => new
                    {
                        x.Account,
                        LastMessageDates = y
                    })
                .Select(x => new
                {
                    x.Account,
                    x.LastMessageDates!.LastMessageDate
                })
                .GroupJoin(unreadMessageCounts, x => x.Account.Id, x => x.AccountId, (x, y) => new
                {
                    Account = x,
                    UnreadMessageCounts = y
                })
                .SelectMany(
                    x => x.UnreadMessageCounts.DefaultIfEmpty(),
                    (x, y) => new
                    {
                        AccountInfo = x.Account,
                        UnreadMessageCounts = y
                    })
                .Select(x => new AccountWithUnreadCount
                {
                    Id = x.AccountInfo.Account.Id,
                    FirstName = x.AccountInfo.Account.FirstName,
                    LastName = x.AccountInfo.Account.LastName,
                    PhotoUrl = x.AccountInfo.Account.PhotoUrl,
                    AccountTypeId = x.AccountInfo.Account.AccountTypeId,
                    LastMessageDate = x.AccountInfo.LastMessageDate,
                    UnreadCount = x.UnreadMessageCounts!.UnreadCount
                })
                .ToListAsync();
        }

        public Task<Account?> GetByIdAsync(int id)
        {
            return _context.Accounts
                .AsNoTracking()
                .SingleOrDefaultAsync(account => account.Id == id);
        }

        public Task UpdateAsync(int accountId, string? firstName, string? lastName, string? photoUrl)
        {
            return _context.Accounts
                .Where(account => account.Id == accountId)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(account => account.FirstName, firstName)
                    .SetProperty(account => account.LastName, lastName)
                    .SetProperty(account => account.PhotoUrl, photoUrl));
        }
    }
}

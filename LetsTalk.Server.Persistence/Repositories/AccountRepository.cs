using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.Repositories;

public class AccountRepository : GenericRepository<Account>, IAccountRepository
{
    public AccountRepository(LetsTalkDbContext context) : base(context)
    {
    }

    public IQueryable<Account> GetByExternalId(string externalId, AccountTypes accountTypes)
    {
        return _context.Accounts
            .Where(q => q.ExternalId == externalId && q.AccountTypeId == (int)accountTypes);
    }

    public IQueryable<Account> GetById(int id, bool includeFile = false)
    {
        return includeFile
            ? _context.Accounts
                .Include(x => x.Image)
                .Include(x => x.Image!.File)
                .Where(q => q.Id == id)
            : _context.Accounts.Where(q => q.Id == id);
    }

    public async Task<IReadOnlyList<AccountWithUnreadCount>> GetOthersAsync(int id, CancellationToken cancellationToken = default)
    {
        var lastMessageDates = _context.Messages
            .Where(x => x.SenderId == id || x.RecipientId == id)
            .GroupBy(x => new
            {
                x.RecipientId,
                x.SenderId
            })
            .Select(g => new
            {
                AccountId = g.Key.RecipientId == id ? g.Key.SenderId : g.Key.RecipientId,
                LastMessageDate = g.Max(x => x.DateCreatedUnix),
                UnreadCount = g.Count(x => g.Key.RecipientId == id && !x.IsRead)
            })
            .GroupBy(g => g.AccountId)
            .Select(g => new
            {
                AccountId = g.Key,
                LastMessageDate = g.Max(x => x.LastMessageDate),
                UnreadCount = g.Sum(x => x.UnreadCount)
            });

        return await _context.Accounts
            .Where(account => account.Id != id)
            .GroupJoin(lastMessageDates, x => x.Id, x => x.AccountId, (x, y) => new
            {
                Account = x,
                Metrics = y
            })
            .SelectMany(
                x => x.Metrics.DefaultIfEmpty(),
                (x, y) => new
                {
                    x.Account,
                    Metrics = y
                })
            .Select(x => new AccountWithUnreadCount
            {
                Id = x.Account.Id,
                FirstName = x.Account.FirstName,
                LastName = x.Account.LastName,
                PhotoUrl = x.Account.PhotoUrl,
                AccountTypeId = x.Account.AccountTypeId,
                LastMessageDate = x.Metrics!.LastMessageDate,
                UnreadCount = x.Metrics.UnreadCount,
                ImageId = x.Account.ImageId
            })
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public Task SetImageIdAsync(int accountId, int imageId, CancellationToken cancellationToken = default)
    {
        return _context.Accounts
            .Where(account => account.Id == accountId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(account => account.PhotoUrl, (string?)null)
                .SetProperty(account => account.ImageId, imageId), cancellationToken: cancellationToken);
    }

    public Task UpdateAsync(int accountId, string? firstName, string? lastName, string? email, CancellationToken cancellationToken = default)
    {
        return _context.Accounts
            .Where(account => account.Id == accountId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(account => account.FirstName, firstName)
                .SetProperty(account => account.LastName, lastName)
                .SetProperty(account => account.Email, email), cancellationToken: cancellationToken);
    }

    public Task UpdateAsync(int accountId, string? firstName, string? lastName, string? email, string? photoUrl, CancellationToken cancellationToken = default)
    {
        return _context.Accounts
            .Where(account => account.Id == accountId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(account => account.FirstName, firstName)
                .SetProperty(account => account.LastName, lastName)
                .SetProperty(account => account.Email, email)
                .SetProperty(account => account.PhotoUrl, photoUrl), cancellationToken: cancellationToken);
    }
}

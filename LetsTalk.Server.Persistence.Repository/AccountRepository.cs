using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LetsTalk.Server.Persistence.Repository;

public class AccountRepository : GenericRepository<Account>, IAccountRepository
{
    public AccountRepository(LetsTalkDbContext context) : base(context)
    {
    }

    public Task<Account> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Accounts
            .FirstOrDefaultAsync(account => account.Id == id, cancellationToken: cancellationToken)!;
    }

    public override Task<Account> GetByIdAsTrackingAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Accounts
            .AsTracking()
            .FirstOrDefaultAsync(account => account.Id == id, cancellationToken: cancellationToken)!;
    }

    public Task<T?> GetByIdAsync<T>(int id, Expression<Func<Account, T>> selector, CancellationToken cancellationToken = default)
    {
        return _context.Accounts
            .Include(account => account.Image)
            .ThenInclude(image => image!.File)
            .Where(account => account.Id == id)
            .Select(selector)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public Task<Account> GetByExternalIdAsync(string externalId, AccountTypes accountType, CancellationToken cancellationToken = default)
    {
        return _context.Accounts
            .FirstOrDefaultAsync(q => q.ExternalId == externalId && q.AccountTypeId == (int)accountType, cancellationToken: cancellationToken)!;
    }

    public Task<Account> GetByExternalIdAsTrackingAsync(string externalId, AccountTypes accountType, CancellationToken cancellationToken = default)
    {
        return _context.Accounts
            .AsTracking()
            .FirstOrDefaultAsync(q => q.ExternalId == externalId && q.AccountTypeId == (int)accountType, cancellationToken: cancellationToken)!;
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
            .Select(x => new AccountWithUnreadCount(
                x.Account.Id,
                x.Account.FirstName,
                x.Account.LastName,
                x.Account.PhotoUrl,
                x.Account.AccountTypeId,
                x.Metrics!.LastMessageDate,
                x.Metrics.UnreadCount,
                x.Account.ImageId))
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Accounts
            .AnyAsync(account => account.Id == id, cancellationToken: cancellationToken);
    }
}

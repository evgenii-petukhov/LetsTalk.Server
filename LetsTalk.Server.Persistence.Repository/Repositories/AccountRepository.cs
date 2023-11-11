using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using LetsTalk.Server.Persistence.Repository.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LetsTalk.Server.Persistence.Repository.Repositories;

public class AccountRepository : GenericRepository<Account>, IAccountRepository
{
    public AccountRepository(LetsTalkDbContext context) : base(context)
    {
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

    public Task<List<AccountListItem>> GetContactsAsync(int id, CancellationToken cancellationToken = default)
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
                LastMessageId = g.Max(x => x.Id),
                UnreadCount = g.Count(x => g.Key.RecipientId == id && !x.IsRead)
            })
            .GroupBy(g => g.AccountId)
            .Select(g => new
            {
                AccountId = g.Key,
                LastMessageDate = g.Max(x => x.LastMessageDate),
                LastMessageId = g.Max(x => x.LastMessageId),
                UnreadCount = g.Sum(x => x.UnreadCount)
            });

        return _context.Accounts
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
            .Select(x => new AccountListItem
            {
                Id = x.Account.Id,
                FirstName = x.Account.FirstName,
                LastName = x.Account.LastName,
                PhotoUrl = x.Account.PhotoUrl,
                AccountTypeId = x.Account.AccountTypeId,
                LastMessageDate = x.Metrics!.LastMessageDate,
                LastMessageId = x.Metrics!.LastMessageId,
                UnreadCount = x.Metrics.UnreadCount,
                ImageId = x.Account.ImageId
            })
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Accounts
            .AnyAsync(account => account.Id == id, cancellationToken: cancellationToken);
    }
}

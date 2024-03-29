﻿using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public class AccountRepository(LetsTalkDbContext context) : GenericRepository<Account>(context), IAccountRepository
{
    public override Task<Account> GetByIdAsTrackingAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Accounts
            .Include(x => x.Image)
            .AsTracking()
            .FirstOrDefaultAsync(account => account.Id == id, cancellationToken)!;
    }

    public Task<Account> GetByExternalIdAsync(string externalId, AccountTypes accountType, CancellationToken cancellationToken = default)
    {
        return _context.Accounts
            .FirstOrDefaultAsync(q => q.ExternalId == externalId && q.AccountTypeId == (int)accountType, cancellationToken)!;
    }

    public Task<Account> GetByExternalIdAsTrackingAsync(string externalId, AccountTypes accountType, CancellationToken cancellationToken = default)
    {
        return _context.Accounts
            .AsTracking()
            .FirstOrDefaultAsync(q => q.ExternalId == externalId && q.AccountTypeId == (int)accountType, cancellationToken)!;
    }

    public Task<List<Contact>> GetContactsAsync(int id, CancellationToken cancellationToken = default)
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
                AccountId = g.Key.RecipientId + g.Key.SenderId - id,
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
            .Select(x => new Contact
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
            .ToListAsync(cancellationToken);
    }

    public Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Accounts
            .AnyAsync(account => account.Id == id, cancellationToken);
    }
}

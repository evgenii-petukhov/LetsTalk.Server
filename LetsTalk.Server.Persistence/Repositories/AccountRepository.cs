using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LetsTalk.Server.Persistence.Repositories;

public class AccountRepository : GenericRepository<Account>, IAccountRepository
{
    public AccountRepository(LetsTalkDbContext context) : base(context)
    {
    }

    public async Task<int> CreateOrUpdateAsync(string externalId, AccountTypes accountType, string? firstName, string? lastName,
        string? email, string? photoUrl, CancellationToken cancellationToken = default)
    {
        var response = await GetByExternalIdOrDefaultAsync(externalId, accountType, x => new
        {
            x.Id,
            x.ImageId
        }, cancellationToken);

        if (response == null)
        {
            try
            {
                var account = new Account
                {
                    ExternalId = externalId,
                    AccountTypeId = (int)accountType,
                    FirstName = firstName,
                    LastName = lastName,
                    PhotoUrl = photoUrl,
                    Email = email
                };
                await CreateAsync(account, cancellationToken);
                return account.Id;
            }
            catch (DbUpdateException)
            {
                return await GetByExternalIdOrDefaultAsync(externalId, accountType, x => x.Id, cancellationToken);
            }
        }
        else
        {
            await (response.ImageId.HasValue
                ? UpdateAsync(response.Id, firstName, lastName, email, cancellationToken)
                : UpdateAsync(response.Id, firstName, lastName, email, photoUrl, cancellationToken));

            return response.Id;
        }
    }

    public Task<T?> GetByIdOrDefaultAsync<T>(int id, Expression<Func<Account, T>> selector, bool includeFile = false, CancellationToken cancellationToken = default)
    {
        return GetById(id, includeFile)
            .Select(selector)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
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

    public Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default)
    {
        return GetById(id)
            .AnyAsync(cancellationToken: cancellationToken);
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

    public Task UpdateAsync(int accountId, string? firstName, string? lastName, string? email, int? imageId, CancellationToken cancellationToken = default)
    {
        return _context.Accounts
            .Where(account => account.Id == accountId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(account => account.FirstName, firstName)
                .SetProperty(account => account.LastName, lastName)
                .SetProperty(account => account.Email, email)
                .SetProperty(account => account.ImageId, imageId), cancellationToken: cancellationToken);
    }

    private IQueryable<Account> GetById(int id, bool includeFile = false)
    {
        return includeFile
            ? _context.Accounts
                .Include(account => account.Image)
                .ThenInclude(image => image!.File)
                .Where(account => account.Id == id)
            : _context.Accounts.Where(account => account.Id == id);
    }

    private Task<T?> GetByExternalIdOrDefaultAsync<T>(string externalId, AccountTypes accountType, Expression<Func<Account, T>> selector, CancellationToken cancellationToken = default)
    {
        return _context.Accounts
            .Where(q => q.ExternalId == externalId && q.AccountTypeId == (int)accountType)
            .Select(selector)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    private Task UpdateAsync(int accountId, string? firstName, string? lastName, string? email, string? photoUrl, CancellationToken cancellationToken = default)
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

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
        return Context.Accounts
            .Include(x => x.Image)
            .AsTracking()
            .FirstOrDefaultAsync(account => account.Id == id, cancellationToken)!;
    }

    public Task<Account> GetByExternalIdAsync(string externalId, AccountTypes accountType, CancellationToken cancellationToken = default)
    {
        return Context.Accounts
            .FirstOrDefaultAsync(q => q.ExternalId == externalId && q.AccountTypeId == (int)accountType, cancellationToken)!;
    }

    public Task<Account> GetByExternalIdAsTrackingAsync(string externalId, AccountTypes accountType, CancellationToken cancellationToken = default)
    {
        return Context.Accounts
            .AsTracking()
            .FirstOrDefaultAsync(q => q.ExternalId == externalId && q.AccountTypeId == (int)accountType, cancellationToken)!;
    }

    public Task<List<Account>> GetAccountsAsync(CancellationToken cancellationToken = default)
    {
        return Context.Accounts.ToListAsync(cancellationToken);
    }

    public Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default)
    {
        return Context.Accounts
            .AnyAsync(account => account.Id == id, cancellationToken);
    }

    public Task<Account> GetByEmailAsync(string email, AccountTypes accountType, CancellationToken cancellationToken = default)
    {
        return Context.Accounts
            .FirstOrDefaultAsync(account => account.Email == email && account.AccountTypeId == (int)accountType, cancellationToken)!;
    }
}

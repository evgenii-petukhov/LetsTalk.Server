﻿using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Utility;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.DatabaseContext;

public class LetsTalkDbContext(DbContextOptions<LetsTalkDbContext> options) : DbContext(options)
{
    public DbSet<AccountType> AccountTypes { get; set; }

    public DbSet<Account> Accounts { get; set; }

    public DbSet<Message> Messages { get; set; }

    public DbSet<LinkPreview> LinkPreviews { get; set; }

    public DbSet<Image> Images { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LetsTalkDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in base.ChangeTracker.Entries<Message>().Where(q => q.State == EntityState.Added))
        {
            entry.Entity.SetDateCreatedUnix(DateHelper.GetUnixTimestamp());
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}

using LetsTalk.Server.Domain;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.DatabaseContext;

public class LetsTalkDbContext : DbContext
{
	public LetsTalkDbContext(DbContextOptions<LetsTalkDbContext> options) : base(options)
    {

	}

    public DbSet<AccountType> AccountTypes { get; set; }

    public DbSet<Account> Accounts { get; set; }

    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LetsTalkDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in base.ChangeTracker.Entries<Message>().Where(q => q.State == EntityState.Added))
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.DateCreated = DateTime.UtcNow;
                entry.Entity.DateCreatedUnix = ConvertToUnixTimestamp(DateTime.Now);
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    private long ConvertToUnixTimestamp(DateTime date)
    {
        DateTime origin = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = date.ToUniversalTime() - origin;
        return (long)diff.TotalSeconds;
    }
}

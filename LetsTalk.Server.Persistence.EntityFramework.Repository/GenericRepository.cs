using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public abstract class GenericRepository<T>(LetsTalkDbContext context)
    : Repository(context), IGenericRepository<T>
    where T : BaseEntity
{
    public async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await Context.Set<T>().AddAsync(entity, cancellationToken);
    }

    public virtual Task<T> GetByIdAsTrackingAsync(int id, CancellationToken cancellationToken = default)
    {
        return Context.Set<T>()
            .AsTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)!;
    }

    public virtual Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return Context.Set<T>()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)!;
    }
}

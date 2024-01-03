using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public abstract class GenericRepository<T> : Repository, IGenericRepository<T>
    where T : BaseEntity
{
    protected GenericRepository(LetsTalkDbContext context) : base(context)
    {
    }

    public async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _context.Set<T>().AddAsync(entity, cancellationToken);
    }

    public virtual Task<T> GetByIdAsTrackingAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Set<T>()
            .AsTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)!;
    }

    public Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Set<T>()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)!;
    }
}

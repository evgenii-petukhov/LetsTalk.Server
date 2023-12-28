using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public class GenericRepository<T> : IGenericRepository<T>, IDisposable
    where T : BaseEntity
{
    protected readonly LetsTalkDbContext _context;
    private bool _disposed;

    public GenericRepository(LetsTalkDbContext context)
    {
        _context = context;
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

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.Repository;

public class GenericRepository<T> : IGenericRepository<T>, IDisposable
    where T : BaseEntity
{
    protected readonly LetsTalkDbContext _context;
    private bool _disposedValue;

    public GenericRepository(LetsTalkDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _context.Set<T>().AddAsync(entity, cancellationToken);
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public virtual Task<T> GetByIdAsTrackingAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Set<T>()
            .AsTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken: cancellationToken)!;
    }

    public virtual Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Set<T>()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken: cancellationToken)!;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

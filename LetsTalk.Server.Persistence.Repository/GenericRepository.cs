using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.Repository.Abstractions;

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
        try
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return;
        }
        catch
        {
            _context.ChangeTracker.Clear();
            throw;
        }
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

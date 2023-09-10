using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.Repository;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly LetsTalkDbContext _context;
    private bool _disposedValue;

    public UnitOfWork(LetsTalkDbContext context)
    {
        _context = context;
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
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
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

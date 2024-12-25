using LetsTalk.Server.Persistence.DatabaseContext;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public abstract class Repository(LetsTalkDbContext context) : IDisposable
{
    protected readonly LetsTalkDbContext _context = context;
    private bool _disposed;

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

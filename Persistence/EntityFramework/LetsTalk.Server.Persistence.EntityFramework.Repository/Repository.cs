using LetsTalk.Server.Persistence.DatabaseContext;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public abstract class Repository(LetsTalkDbContext context) : IDisposable
{
    protected LetsTalkDbContext Context { get; } = context;
    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Context.Dispose();
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

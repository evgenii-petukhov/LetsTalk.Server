﻿using LetsTalk.Server.Persistence.DatabaseContext;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public abstract class Repository: IDisposable
{
    protected readonly LetsTalkDbContext _context;
    private bool _disposed;

    protected Repository(LetsTalkDbContext context)
    {
        _context = context;
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

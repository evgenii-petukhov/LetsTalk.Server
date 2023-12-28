using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public class ImageRepository : IImageRepository, IDisposable
{
    protected readonly LetsTalkDbContext _context;
    private bool _disposed;

    public ImageRepository(LetsTalkDbContext context)
    {
        _context = context;
    }

    public void Delete(Image image)
    {
        _context.Images.Remove(image);
    }

    private void Dispose(bool disposing)
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
using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LetsTalk.Server.Persistence.Repository;

public class ImageRepository : GenericRepository<Image>, IImageRepository
{
    public ImageRepository(LetsTalkDbContext context) : base(context)
    {
    }

    public Task<T?> GetByIdAsync<T>(int id, Expression<Func<Image, T>> selector, CancellationToken cancellationToken = default)
    {
        return _context.Images
            .Include(x => x.File)
            .Where(image => image.Id == id)
            .Select(selector)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public override Task<Image> GetByIdAsTrackingAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Images
            .AsTracking()
            .Include(x => x.File)
            .FirstOrDefaultAsync(image => image.Id == id, cancellationToken)!;
    }

    public Task<bool> IsImageIdValidAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Images
            .AnyAsync(image => image.Id == id, cancellationToken: cancellationToken);
    }
}
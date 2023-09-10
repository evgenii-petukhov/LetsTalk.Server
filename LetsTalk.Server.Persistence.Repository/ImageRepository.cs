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

    public Task<T?> GetByIdOrDefaultAsync<T>(int id, Expression<Func<Image, T>> selector, CancellationToken cancellationToken = default)
    {
        return _context.Images
            .Include(x => x.File)
            .Where(image => image.Id == id)
            .Select(selector)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> IsImageIdValidAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Images
            .AnyAsync(image => image.Id == id, cancellationToken: cancellationToken);
    }

    public Task SetDimensionsAsync(int imageId, int width, int height, CancellationToken cancellationToken = default)
    {
        return _context.Images
            .Where(image => image.Id == imageId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(image => image.Width, width)
                .SetProperty(image => image.Height, height), cancellationToken: cancellationToken);
    }
}
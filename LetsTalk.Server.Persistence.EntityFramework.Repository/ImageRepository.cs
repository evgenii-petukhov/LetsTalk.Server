using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public class ImageRepository : GenericRepository<Image>, IImageRepository
{
    public ImageRepository(LetsTalkDbContext context) : base(context)
    {
    }

    public Task<Image?> GetByIdWithFileAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Images
            .Include(x => x.File)
            .FirstOrDefaultAsync(image => image.Id == id, cancellationToken);
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
            .AnyAsync(image => image.Id == id, cancellationToken);
    }
}
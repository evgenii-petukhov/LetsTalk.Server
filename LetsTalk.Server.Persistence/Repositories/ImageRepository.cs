using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.Repositories;

public class ImageRepository : GenericRepository<Image>, IImageRepository
{
    public ImageRepository(LetsTalkDbContext context) : base(context)
    {
    }

    public Task<Image?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Images
            .SingleOrDefaultAsync(image => image.Id == id, cancellationToken: cancellationToken);
    }

    public Task<Image?> GetByIdWithFileAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Images
            .Include(x => x.File)
            .SingleOrDefaultAsync(image => image.Id == id, cancellationToken: cancellationToken);
    }
}

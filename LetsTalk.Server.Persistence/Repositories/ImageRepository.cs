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

    public IQueryable<Image> GetById(int id)
    {
        return _context.Images
            .Include(x => x.File)
            .Where(image => image.Id == id);
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

using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public abstract class ImageRepository : Repository, IImageRepository
{
    protected ImageRepository(LetsTalkDbContext context) : base(context)
    {
    }

    public void Delete(Image image)
    {
        _context.Images.Remove(image);
    }
}
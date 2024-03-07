using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository;

public class ImageRepository(LetsTalkDbContext context) : Repository(context), IImageRepository
{
    public void Delete(Image image)
    {
        _context.Images.Remove(image);
    }
}
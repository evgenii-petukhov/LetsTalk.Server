using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.Abstractions;

public interface IImageRepository : IGenericRepository<Image>
{
    IQueryable<Image> GetById(int id);

    Task SetDimensionsAsync(int imageId, int width, int height, CancellationToken cancellationToken = default);
}

using LetsTalk.Server.Domain;
using System.Linq.Expressions;

namespace LetsTalk.Server.Persistence.Repository.Abstractions;

public interface IImageRepository : IGenericRepository<Image>
{
    Task<T?> GetByIdOrDefaultAsync<T>(int id, Expression<Func<Image, T>> selector, CancellationToken cancellationToken = default);

    Task SetDimensionsAsync(int imageId, int width, int height, CancellationToken cancellationToken = default);
}

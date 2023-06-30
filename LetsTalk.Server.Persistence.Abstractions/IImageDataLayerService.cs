using LetsTalk.Server.Domain;
using System.Linq.Expressions;

namespace LetsTalk.Server.Persistence.Abstractions;

public interface IImageDataLayerService
{
    Task<T?> GetByIdOrDefaultAsync<T>(int id, Expression<Func<Image, T>> selector, CancellationToken cancellationToken = default);
}

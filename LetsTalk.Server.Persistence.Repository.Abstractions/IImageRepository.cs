using LetsTalk.Server.Domain;
using System.Linq.Expressions;

namespace LetsTalk.Server.Persistence.Repository.Abstractions;

public interface IImageRepository : IGenericRepository<Image>
{
    Task<T?> GetByIdWithFileAsync<T>(int id, Expression<Func<Image, T>> selector, CancellationToken cancellationToken = default);

    Task<bool> IsImageIdValidAsync(int id, CancellationToken cancellationToken = default);
}

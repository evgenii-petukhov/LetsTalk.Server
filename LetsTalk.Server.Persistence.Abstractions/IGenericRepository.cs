using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.Abstractions;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task CreateAsync(T entity, CancellationToken cancellationToken = default);
}

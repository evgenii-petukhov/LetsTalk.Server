using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.Repository.Abstractions;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task CreateAsync(T entity, CancellationToken cancellationToken = default);
}

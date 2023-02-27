using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Abstractions.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T> CreateAsync(T entity);
    }
}

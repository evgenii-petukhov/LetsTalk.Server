using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Abstractions.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IReadOnlyList<T>> GetAsync();

        Task<T> GetByIdAsync(int id);

        Task<T> CreateAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);
    }
}

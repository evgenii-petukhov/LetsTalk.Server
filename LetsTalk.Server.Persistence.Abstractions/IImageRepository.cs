using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.Abstractions;

public interface IImageRepository : IGenericRepository<Image>
{
    Task<Image?> GetByIdAsync(int id);

    Task DeleteAsync(int id);
}

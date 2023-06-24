using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.Abstractions;

public interface IImageRepository : IGenericRepository<Image>
{
    Task<Image?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Image?> GetByIdWithFileAsync(int id, CancellationToken cancellationToken = default);
}

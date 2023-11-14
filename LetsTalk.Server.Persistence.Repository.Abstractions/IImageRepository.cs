using LetsTalk.Server.Domain;

namespace LetsTalk.Server.Persistence.Repository.Abstractions;

public interface IImageRepository : IGenericRepository<Image>
{
    Task<Image?> GetByIdWithFileAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> IsImageIdValidAsync(int id, CancellationToken cancellationToken = default);
}

namespace LetsTalk.Server.Persistence.Abstractions;

public interface IFileRepository : IGenericRepository<Domain.File>
{
    Task<Domain.File?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

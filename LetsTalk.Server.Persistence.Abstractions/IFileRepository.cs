namespace LetsTalk.Server.Persistence.Abstractions;

public interface IFileRepository : IGenericRepository<Domain.File>
{
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

namespace LetsTalk.Server.Persistence.Repository.Abstractions;

public interface IFileRepository : IGenericRepository<Domain.File>
{
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

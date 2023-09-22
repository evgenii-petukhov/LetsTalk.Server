namespace LetsTalk.Server.Persistence.Repository.Abstractions;

public interface IUnitOfWork
{
    Task SaveAsync(CancellationToken cancellationToken = default);
}

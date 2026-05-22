namespace LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

public interface IUnitOfWork
{
    Task SaveAsync(CancellationToken cancellationToken = default);
}

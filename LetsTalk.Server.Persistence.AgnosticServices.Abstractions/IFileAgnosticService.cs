namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

public interface IFileAgnosticService
{
    Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default);
}

namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

public interface IFileAgnosticService
{
    Task DeleteByIdAsync(string id, CancellationToken cancellationToken = default);
}

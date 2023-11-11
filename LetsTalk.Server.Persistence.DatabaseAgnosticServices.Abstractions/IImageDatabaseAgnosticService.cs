namespace LetsTalk.Server.Persistence.DatabaseAgnosticServices.Abstractions;

public interface IImageDatabaseAgnosticService
{
    Task<bool> IsImageIdValidAsync(int id, CancellationToken cancellationToken = default);
}

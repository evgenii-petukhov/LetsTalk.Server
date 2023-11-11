namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

public interface IImageAgnosticService
{
    Task<bool> IsImageIdValidAsync(int id, CancellationToken cancellationToken = default);
}

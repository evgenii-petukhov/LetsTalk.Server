namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

public interface IAccountAgnosticService
{
    Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default);
}

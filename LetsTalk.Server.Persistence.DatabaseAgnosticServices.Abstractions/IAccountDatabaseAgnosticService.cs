namespace LetsTalk.Server.Persistence.DatabaseAgnosticServices.Abstractions;

public interface IAccountDatabaseAgnosticService
{
    Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default);
}

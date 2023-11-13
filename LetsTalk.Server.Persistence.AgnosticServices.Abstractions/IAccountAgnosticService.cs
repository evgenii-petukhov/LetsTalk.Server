using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

public interface IAccountAgnosticService
{
    Task<bool> IsAccountIdValidAsync(int id, CancellationToken cancellationToken = default);

    Task<AccountServiceModel> UpdateProfileAsync(
        int accountId,
        string firstName,
        string lastName,
        string email,
        int? imageId,
        CancellationToken cancellationToken = default);
}

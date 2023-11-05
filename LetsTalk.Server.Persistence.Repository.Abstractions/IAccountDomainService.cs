using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace LetsTalk.Server.Persistence.Repository.Abstractions;

public interface IAccountDomainService
{
    Task<Account> CreateAccountAsync(
        string externalId,
        AccountTypes accountType,
        string firstName,
        string lastName,
        string email,
        string photoUrl,
        CancellationToken cancellationToken = default);

    Task<Account> UpdateProfileAsync(
        int accountId,
        string firstName,
        string lastName,
        string email,
        int? imageId,
        CancellationToken cancellationToken = default);
}

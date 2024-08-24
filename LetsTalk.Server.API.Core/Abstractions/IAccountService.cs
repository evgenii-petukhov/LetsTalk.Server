using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.API.Core.Abstractions;

public interface IAccountService
{
    Task<IReadOnlyList<AccountDto>> GetAccountsAsync(CancellationToken cancellationToken);
}

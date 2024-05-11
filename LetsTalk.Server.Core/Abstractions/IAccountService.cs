using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IAccountService
{
    Task<IReadOnlyList<AccountDto>> GetAccountsAsync(CancellationToken cancellationToken);
}

using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IAccountService
{
    Task<List<AccountDto>> GetAccountsAsync(string id, CancellationToken cancellationToken);
}

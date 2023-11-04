using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IAccountService
{
    Task<List<AccountDto>> GetContactsAsync(int accountId, CancellationToken cancellationToken);
}

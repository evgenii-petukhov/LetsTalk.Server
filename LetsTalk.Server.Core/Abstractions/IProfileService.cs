using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IProfileService
{
    Task<AccountDto> GetProfileAsync(int accountId, CancellationToken cancellationToken);
}

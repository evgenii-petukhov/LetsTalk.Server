using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IProfileService
{
    Task<AccountDto> GetProfileAsync(string accountId, CancellationToken cancellationToken);
}

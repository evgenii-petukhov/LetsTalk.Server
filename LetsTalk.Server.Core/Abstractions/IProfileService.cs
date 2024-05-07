using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IProfileService
{
    Task<ProfileDto> GetProfileAsync(string accountId, CancellationToken cancellationToken);
}

using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.API.Core.Abstractions;

public interface IProfileService
{
    Task<ProfileDto> GetProfileAsync(string accountId, CancellationToken cancellationToken);
}

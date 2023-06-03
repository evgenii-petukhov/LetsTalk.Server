using LetsTalk.Server.API.Models.Login;
using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IVkService
{
    Task<LoginResponseDto> LoginAsync(LoginServiceInput model, CancellationToken cancellationToken);
}

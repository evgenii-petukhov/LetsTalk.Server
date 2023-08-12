using LetsTalk.Server.API.Models.Login;
using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IOpenAuthProvider
{
    Task<LoginResponseDto> LoginAsync(LoginServiceInput model, CancellationToken cancellationToken);
}

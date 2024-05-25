using LetsTalk.Server.Core.Features.Authentication.Commands.Login;
using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.Abstractions;

public interface IOpenAuthProvider
{
    Task<LoginResponseDto> LoginAsync(LoginCommand model, CancellationToken cancellationToken);
}

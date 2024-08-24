using LetsTalk.Server.API.Core.Features.Authentication.Commands.Login;
using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.API.Core.Abstractions;

public interface IOpenAuthProvider
{
    Task<LoginResponseDto> LoginAsync(LoginCommand model, CancellationToken cancellationToken);
}

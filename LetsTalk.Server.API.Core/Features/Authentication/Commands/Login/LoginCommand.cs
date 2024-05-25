using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.Features.Authentication.Commands.Login;

public class LoginCommand : MediatR.IRequest<LoginResponseDto>
{
    public string? Provider { get; set; }

    public string? Id { get; set; }

    public string? AuthToken { get; set; }
}

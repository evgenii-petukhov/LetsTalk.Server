using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Authentication.Commands.LoginByEmail;

public class LoginByEmailCommandHandler() : IRequestHandler<LoginByEmailCommand, LoginResponseDto>
{
    public Task<LoginResponseDto> Handle(LoginByEmailCommand command, CancellationToken cancellationToken)
    {
        return Task.FromResult(new LoginResponseDto
        {
            Success = true,
            Token = string.Empty
        });
    }
}

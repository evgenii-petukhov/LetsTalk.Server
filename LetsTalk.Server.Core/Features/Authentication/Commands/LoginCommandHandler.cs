using LetsTalk.Server.Core.Models.Authentication;
using MediatR;

namespace LetsTalk.Server.Core.Features.Authentication.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    public Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new LoginResponse());
    }
}

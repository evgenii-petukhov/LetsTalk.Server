using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.Authentication.Commands.Login;

public class LoginCommandHandler(
    IOpenAuthProviderResolver<string> openAuthProviderResolver) : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IOpenAuthProviderResolver<string> _openAuthProviderResolver = openAuthProviderResolver;

    public Task<LoginResponseDto> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var authProvider = _openAuthProviderResolver.Resolve(command.Provider!);
        return authProvider.LoginAsync(command, cancellationToken);
    }
}

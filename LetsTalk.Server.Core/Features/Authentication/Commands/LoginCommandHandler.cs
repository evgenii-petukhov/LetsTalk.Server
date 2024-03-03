using AutoMapper;
using LetsTalk.Server.API.Models.Login;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Authentication.Commands;

public class LoginCommandHandler(
    IOpenAuthProviderResolver<string> openAuthProviderResolver,
    IMapper mapper) : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IOpenAuthProviderResolver<string> _openAuthProviderResolver = openAuthProviderResolver;
    private readonly IMapper _mapper = mapper;

    public Task<LoginResponseDto> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var model = _mapper.Map<LoginServiceInput>(command);
        var authProvider = _openAuthProviderResolver.Resolve(command.Provider!);
        return authProvider.LoginAsync(model, cancellationToken);
    }
}

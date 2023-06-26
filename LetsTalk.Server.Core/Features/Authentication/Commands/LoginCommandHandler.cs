using AutoMapper;
using LetsTalk.Server.API.Models.Login;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Authentication.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;

    public LoginCommandHandler(
        IAuthenticationService authenticationService,
        IMapper mapper)
    {
        _authenticationService = authenticationService;
        _mapper = mapper;
    }

    public Task<LoginResponseDto> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var model = _mapper.Map<LoginServiceInput>(command);
        return _authenticationService.LoginAsync(model, cancellationToken);
    }
}

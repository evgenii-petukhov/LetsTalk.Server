using AutoMapper;
using LetsTalk.Server.Abstractions.Authentication;
using LetsTalk.Server.Models.Authentication;
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

    public async Task<LoginResponseDto> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var model = _mapper.Map<LoginServiceInput>(command);
        return await _authenticationService.Login(model);
    }
}

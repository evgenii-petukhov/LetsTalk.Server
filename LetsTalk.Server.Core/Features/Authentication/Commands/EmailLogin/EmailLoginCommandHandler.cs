using AutoMapper;
using LetsTalk.Server.API.Models.Login;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Authentication.Commands.EmailLogin;

public class EmailLoginCommandHandler(
    IEmailLoginService emailLoginService,
    IMapper mapper) : IRequestHandler<EmailLoginCommand, LoginResponseDto>
{
    private readonly IEmailLoginService _emailLoginService = emailLoginService;
    private readonly IMapper _mapper = mapper;

    public Task<LoginResponseDto> Handle(EmailLoginCommand command, CancellationToken cancellationToken)
    {
        var model = _mapper.Map<EmailLoginServiceModel>(command);
        return _emailLoginService.LoginAsync(model, cancellationToken);
    }
}

using AutoMapper;
using LetsTalk.Server.API.Models.Login;
using LetsTalk.Server.Core.Features.Authentication.Commands.Login;
using LetsTalk.Server.Core.Features.Authentication.Commands.EmailLogin;
using LetsTalk.Server.Dto.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Server.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController(
    IMediator mediator,
    IMapper mapper) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> LoginAsync(LoginRequest model, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(_mapper.Map<LoginCommand>(model), cancellationToken);

        return Ok(result);
    }

    [HttpPost("email-login")]
    public async Task<ActionResult<LoginResponseDto>> EmailLoginAsync(EmailLoginRequest model, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(_mapper.Map<EmailLoginCommand>(model), cancellationToken);

        return Ok(result);
    }

    [HttpPost("generate-login-code")]
    public async Task<ActionResult> GenerateLoginCodeAsync(string email, CancellationToken cancellationToken)
    {
        await _mediator.Send(new GenerateLoginCodeCommand(email), cancellationToken);

        return Ok();
    }
}

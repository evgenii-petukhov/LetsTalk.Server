using AutoMapper;
using LetsTalk.Server.API.Models.Login;
using LetsTalk.Server.API.Core.Features.Authentication.Commands.Login;
using LetsTalk.Server.API.Core.Features.Authentication.Commands.EmailLogin;
using LetsTalk.Server.Dto.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.API.Validators;

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
        var validator = new EmailLoginRequestValidator();
        var validationResult = await validator.ValidateAsync(model, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid request", validationResult);
        }

        var result = await _mediator.Send(_mapper.Map<EmailLoginCommand>(model), cancellationToken);

        return Ok(result);
    }

    [HttpPost("generate-login-code")]
    public async Task<ActionResult<GenerateLoginCodeResponseDto>> GenerateLoginCodeAsync(GenerateLoginCodeRequest model, CancellationToken cancellationToken)
    {
        var validator = new GenerateLoginCodeRequestValidator();
        var validationResult = await validator.ValidateAsync(model, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid request", validationResult);
        }

        var result = await _mediator.Send(new GenerateLoginCodeCommand(model.Email!), cancellationToken);

        return Ok(result);
    }
}

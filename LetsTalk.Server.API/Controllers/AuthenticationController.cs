using AutoMapper;
using LetsTalk.Server.API.Models.Login;
using LetsTalk.Server.API.Core.Features.Authentication.Commands.EmailLogin;
using LetsTalk.Server.Dto.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.API.Validators;
using Microsoft.Extensions.Options;
using LetsTalk.Server.Configuration.Models;

namespace LetsTalk.Server.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController(
    IMediator mediator,
    IMapper mapper,
    IOptions<SecuritySettings> securitySettingsOptions) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IMapper _mapper = mapper;
    private readonly SecuritySettings _securitySettings = securitySettingsOptions.Value;

    [HttpPost("email-login")]
    public async Task<ActionResult<LoginResponseDto>> EmailLoginAsync(EmailLoginRequest request, CancellationToken cancellationToken)
    {
        var validator = new EmailLoginRequestValidator(_securitySettings);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid request", validationResult);
        }

        var result = await _mediator.Send(_mapper.Map<EmailLoginCommand>(request), cancellationToken);

        return Ok(result);
    }

    [HttpPost("generate-login-code")]
    public async Task<ActionResult<GenerateLoginCodeResponseDto>> GenerateLoginCodeAsync(GenerateLoginCodeRequest request, CancellationToken cancellationToken)
    {
        var validator = new GenerateLoginCodeRequestValidator(_securitySettings);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid request", validationResult);
        }

        var result = await _mediator.Send(new GenerateLoginCodeCommand(request.Email!), cancellationToken);

        return Ok(result);
    }
}

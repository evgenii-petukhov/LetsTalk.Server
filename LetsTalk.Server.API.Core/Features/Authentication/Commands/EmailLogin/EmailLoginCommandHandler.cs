using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.Authentication.Commands.EmailLogin;

public class EmailLoginCommandHandler(
    IAuthenticationClient authenticationClient,
    IAccountAgnosticService accountAgnosticService,
    ILoginCodeCacheService loginCodeCacheService) : IRequestHandler<EmailLoginCommand, LoginResponseDto>
{
    private readonly IAuthenticationClient _authenticationClient = authenticationClient;
    private readonly IAccountAgnosticService _accountAgnosticService = accountAgnosticService;
    private readonly ILoginCodeCacheService _loginCodeCacheService = loginCodeCacheService;

    public async Task<LoginResponseDto> Handle(EmailLoginCommand command, CancellationToken cancellationToken)
    {
        var validator = new EmailLoginCommandValidator(_loginCodeCacheService);
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid request", validationResult);
        }

        var accountId = await _accountAgnosticService.GetOrCreateAsync(
            AccountTypes.Email,
            command.Email?.Trim().ToLowerInvariant()!,
            cancellationToken);

        var token = await _authenticationClient.GenerateJwtTokenAsync(accountId);

        return new LoginResponseDto
        {
            Success = true,
            Token = token
        };
    }
}

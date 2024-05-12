using LetsTalk.Server.API.Models.Login;
using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Core.Services;

public class EmailLoginService(
    IAuthenticationClient authenticationClient,
    IAccountAgnosticService accountAgnosticService,
    ILoginCodeCacheService loginCodeCacheService) : IEmailLoginService
{
    private readonly IAuthenticationClient _authenticationClient = authenticationClient;
    private readonly IAccountAgnosticService _accountAgnosticService = accountAgnosticService;
    private readonly ILoginCodeCacheService _loginCodeCacheService = loginCodeCacheService;

    public async Task<LoginResponseDto> LoginAsync(EmailLoginServiceModel model, CancellationToken cancellationToken)
    {
        var isCodeValid = await _loginCodeCacheService.ValidateCodeAsync(model.Email!, model.Code);

        if (!isCodeValid)
        {
            throw new BadRequestException();
        }

        var accountId = await _accountAgnosticService.GetOrCreateAsync(
            AccountTypes.Email,
            model.Email!,
            cancellationToken);

        // generate jwt token to access secure routes on this API
        var token = await _authenticationClient.GenerateJwtTokenAsync(accountId);

        return new LoginResponseDto
        {
            Success = true,
            Token = token
        };
    }
}

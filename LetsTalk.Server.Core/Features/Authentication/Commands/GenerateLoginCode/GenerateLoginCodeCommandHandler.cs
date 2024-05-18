using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using MediatR;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Core.Features.Authentication.Commands.EmailLogin;

public class GenerateLoginCodeCommandHandler(
    ILoginCodeCacheService loginCodeCacheService,
    IEmailService emailService,
    IOptions<CachingSettings> options) : IRequestHandler<GenerateLoginCodeCommand, GenerateLoginCodeResponseDto>
{
    private readonly ILoginCodeCacheService _loginCodeCacheService = loginCodeCacheService;
    private readonly IEmailService _emailService = emailService;
    private readonly CachingSettings _cachingSettings = options.Value;

    public async Task<GenerateLoginCodeResponseDto> Handle(GenerateLoginCodeCommand command, CancellationToken cancellationToken)
    {
        var isCodeCreated = await _loginCodeCacheService.GenerateCodeAsync(command.Email!);

        if (isCodeCreated)
        {
            await _emailService.SendAsync(null!, null!, null!, null!, cancellationToken);
        }

        return new GenerateLoginCodeResponseDto
        {
            CodeValidInSeconds = _cachingSettings.LoginCodeCacheLifeTimeInSeconds
        };
    }
}

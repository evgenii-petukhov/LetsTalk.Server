using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using MediatR;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Core.Features.Authentication.Commands.EmailLogin;

public class GenerateLoginCodeCommandHandler(
    ILoginCodeCacheService loginCodeCacheService,
    IOptions<CachingSettings> options) : IRequestHandler<GenerateLoginCodeCommand, GenerateLoginCodeResponseDto>
{
    private readonly ILoginCodeCacheService _loginCodeCacheService = loginCodeCacheService;
    private readonly CachingSettings _cachingSettings = options.Value;

    public async Task<GenerateLoginCodeResponseDto> Handle(GenerateLoginCodeCommand command, CancellationToken cancellationToken)
    {
        await _loginCodeCacheService.GenerateCodeAsync(command.Email!);

        return new GenerateLoginCodeResponseDto
        {
            CodeValidInSeconds = _cachingSettings.LoginCodeCacheLifeTimeInSeconds
        };
    }
}

using LetsTalk.Server.Core.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Authentication.Commands.EmailLogin;

public class GenerateLoginCodeCommandHandler(
    ILoginCodeCacheService loginCodeCacheService) : IRequestHandler<GenerateLoginCodeCommand, Unit>
{
    private readonly ILoginCodeCacheService _loginCodeCacheService = loginCodeCacheService;

    public async Task<Unit> Handle(GenerateLoginCodeCommand command, CancellationToken cancellationToken)
    {
        await _loginCodeCacheService.GenerateCodeAsync(command.Email!);

        return Unit.Value;
    }
}

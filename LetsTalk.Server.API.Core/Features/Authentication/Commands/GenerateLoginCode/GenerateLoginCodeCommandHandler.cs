using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Features.Authentication.Commands.EmailLogin;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Kafka.Models;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.Authentication.Commands.GenerateLoginCode;

public class GenerateLoginCodeCommandHandler(
    ILoginCodeCacheService loginCodeCacheService,
    IProducer<SendLoginCodeRequest> producer
) : IRequestHandler<GenerateLoginCodeCommand, GenerateLoginCodeResponseDto>
{
    private readonly ILoginCodeCacheService _loginCodeCacheService = loginCodeCacheService;
    private readonly IProducer<SendLoginCodeRequest> _producer = producer;

    public async Task<GenerateLoginCodeResponseDto> Handle(GenerateLoginCodeCommand command, CancellationToken cancellationToken)
    {
        var email = command.Email.Trim().ToLowerInvariant();

        var (code, isCodeCreated, ttl) = await _loginCodeCacheService.GenerateCodeAsync(email);

        if (isCodeCreated)
        {
            await _producer.PublishAsync(
                new SendLoginCodeRequest
                {
                    Email = email,
                    Code = code
                }, cancellationToken);
        }

        return new GenerateLoginCodeResponseDto
        {
            CodeValidInSeconds = (int)ttl.TotalSeconds
        };
    }
}

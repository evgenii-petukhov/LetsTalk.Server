using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Kafka.Models;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.Authentication.Commands.GenerateLoginCode;

public class GenerateLoginCodeCommandHandler(
    IAuthenticationClient authenticationClient,
    IProducer<SendLoginCodeRequest> producer
) : IRequestHandler<GenerateLoginCodeCommand, GenerateLoginCodeResponseDto>
{
    private readonly IAuthenticationClient _authenticationClient = authenticationClient;
    private readonly IProducer<SendLoginCodeRequest> _producer = producer;

    public async Task<GenerateLoginCodeResponseDto> Handle(GenerateLoginCodeCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var (code, isCodeCreated, ttl) = await _authenticationClient.GenerateLoginCodeAsync(email);

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
            CodeValidInSeconds = ttl
        };
    }
}

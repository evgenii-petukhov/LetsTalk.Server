using KafkaFlow;
using KafkaFlow.Producers;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Kafka.Models;
using MediatR;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Core.Features.Authentication.Commands.EmailLogin;

public class GenerateLoginCodeCommandHandler : IRequestHandler<GenerateLoginCodeCommand, GenerateLoginCodeResponseDto>
{
    private readonly ILoginCodeCacheService _loginCodeCacheService;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IMessageProducer _sendLoginCodeProducer;

    public GenerateLoginCodeCommandHandler(
        ILoginCodeCacheService loginCodeCacheService,
        IProducerAccessor producerAccessor,
        IOptions<KafkaSettings> kafkaSettings)
    {
        _loginCodeCacheService = loginCodeCacheService;
        _kafkaSettings = kafkaSettings.Value;
        _sendLoginCodeProducer = producerAccessor.GetProducer(_kafkaSettings.SendLoginCodeRequest!.Producer);
    }

    public async Task<GenerateLoginCodeResponseDto> Handle(GenerateLoginCodeCommand command, CancellationToken cancellationToken)
    {
        var email = command.Email.Trim().ToLower();

        var (code, isCodeCreated, ttl) = await _loginCodeCacheService.GenerateCodeAsync(email);

        if (isCodeCreated)
        {
            await _sendLoginCodeProducer.ProduceAsync(
                _kafkaSettings.SendLoginCodeRequest!.Topic,
                Guid.NewGuid().ToString(),
                new SendLoginCodeRequest
                {
                    Email = email,
                    Code = code
                });
        }

        return new GenerateLoginCodeResponseDto
        {
            CodeValidInSeconds = (int)ttl.TotalSeconds
        };
    }
}

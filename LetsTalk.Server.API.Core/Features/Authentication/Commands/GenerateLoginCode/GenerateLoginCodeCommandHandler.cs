using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Kafka.Models;
using MediatR;
using System.Globalization;
using System.Text;

namespace LetsTalk.Server.API.Core.Features.Authentication.Commands.GenerateLoginCode;

public class GenerateLoginCodeCommandHandler(
    IAuthenticationClient authenticationClient,
    IProducer<SendEmailRequest> producer
) : IRequestHandler<GenerateLoginCodeCommand, GenerateLoginCodeResponseDto>
{
    private static readonly CompositeFormat MessageTemplate = CompositeFormat.Parse(Localization.LoginCodeEmailTemplate);

    private readonly IAuthenticationClient _authenticationClient = authenticationClient;
    private readonly IProducer<SendEmailRequest> _producer = producer;

    public async Task<GenerateLoginCodeResponseDto> Handle(GenerateLoginCodeCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var (code, isCodeCreated, ttl) = await _authenticationClient.GenerateLoginCodeAsync(email);

        if (isCodeCreated)
        {
            await _producer.PublishAsync(
                new SendEmailRequest
                {
                    Address = email,
                    Subject = Localization.LoginCodeEmailSubject,
                    Body = string.Format(CultureInfo.InvariantCulture, MessageTemplate, code)
                }, cancellationToken);
        }

        return new GenerateLoginCodeResponseDto
        {
            CodeValidInSeconds = ttl
        };
    }
}

using KafkaFlow;
using LetsTalk.Server.EmailService.Abstractions;
using LetsTalk.Server.Kafka.Models;

namespace LetsTalk.Server.EmailService;

public class SendLoginCodeRequestHandler(IEmailService emailService) : IMessageHandler<SendLoginCodeRequest>
{
    private const string MessageTemplate = "Hi!\r\n" +
        "\r\n" +
        "Your login code is {0}\r\n" +
        "\r\n" +
        "All the best,\r\n" +
        "LetsTalk team.";

    private const string Subject = "LetsTalk: login code";

    private readonly IEmailService _emailService = emailService;

    public Task Handle(IMessageContext context, SendLoginCodeRequest request)
    {
        return _emailService.SendAsync(
            request.Email!,
            null!,
            Subject,
            string.Format(MessageTemplate, request.Code));
    }
}

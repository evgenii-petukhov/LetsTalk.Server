using LetsTalk.Server.EmailService.Abstractions;
using LetsTalk.Server.Kafka.Models;
using MassTransit;
using System.Globalization;
using System.Text;

namespace LetsTalk.Server.EmailService;

public class SendLoginCodeRequestConsumer(IEmailService emailService) : IConsumer<SendLoginCodeRequest>
{
    private static readonly CompositeFormat MessageTemplate = CompositeFormat.Parse("{0} is your new login code\r\n" +
        "\r\n" +
        "All the best,\r\n" +
        "LetsTalk team.");

    private const string Subject = "LetsTalk: login code";

    private readonly IEmailService _emailService = emailService;

    public Task Consume(ConsumeContext<SendLoginCodeRequest> context)
    {
        return _emailService.SendAsync(
            context.Message.Email!,
            null!,
            Subject,
            string.Format(CultureInfo.InvariantCulture, MessageTemplate, context.Message.Code));
    }
}

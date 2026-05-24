using LetsTalk.Server.EmailService.Abstractions;
using LetsTalk.Server.Models.Kafka;
using MassTransit;

namespace LetsTalk.Server.EmailService;

public class SendEmailRequestConsumer(IEmailService emailService) : IConsumer<SendEmailRequest>
{
    private readonly IEmailService _emailService = emailService;

    public Task Consume(ConsumeContext<SendEmailRequest> context)
    {
        return _emailService.SendAsync(
            context.Message.Address!,
            null!,
            context.Message.Subject!,
            context.Message.Body!);
    }
}

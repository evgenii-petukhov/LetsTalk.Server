using LetsTalk.Server.Core.Abstractions;

namespace LetsTalk.Server.Core.Services;

public class EmailService : IEmailService
{
    public Task SendAsync(
        string recipientEmail,
        string senderEmail,
        string subject,
        string text,
        CancellationToken cancellation)
    {
        return Task.CompletedTask;
    }
}

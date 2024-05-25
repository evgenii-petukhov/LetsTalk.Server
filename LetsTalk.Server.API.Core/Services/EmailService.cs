using LetsTalk.Server.API.Core.Abstractions;

namespace LetsTalk.Server.API.Core.Services;

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

namespace LetsTalk.Server.Core.Abstractions;

public interface IEmailService
{
    Task SendAsync(
        string recipientEmail,
        string senderEmail,
        string subject,
        string text,
        CancellationToken cancellation);
}

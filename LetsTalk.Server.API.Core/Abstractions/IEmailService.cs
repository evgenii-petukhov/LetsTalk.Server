namespace LetsTalk.Server.API.Core.Abstractions;

public interface IEmailService
{
    Task SendAsync(
        string recipientEmail,
        string senderEmail,
        string subject,
        string text,
        CancellationToken cancellation);
}

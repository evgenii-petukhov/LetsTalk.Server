namespace LetsTalk.Server.EmailService.Abstractions;

public interface IEmailService
{
    Task SendAsync(string recipientEmail, string recipientName, string subject, string text);
}

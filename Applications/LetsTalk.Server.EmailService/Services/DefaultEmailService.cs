using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.EmailService.Abstractions;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace LetsTalk.Server.EmailService.Services;

public class DefaultEmailService(IOptions<EmailServiceSettings> options) : IEmailService
{
    private readonly EmailServiceSettings _settings = options.Value;

    public async Task SendAsync(string recipientEmail, string recipientName, string subject, string text)
    {
        var emailBodyBuilder = new BodyBuilder
        {
            TextBody = text
        };

        using var message = new MimeMessage
        {
            Subject = subject,
            Body = emailBodyBuilder.ToMessageBody()
        };

        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(new MailboxAddress(recipientName, recipientEmail));

        using var mailClient = new SmtpClient();
        await mailClient.ConnectAsync(_settings.Server, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);
        await mailClient.AuthenticateAsync(_settings.UserName, _settings.Password);
        await mailClient.SendAsync(message);
        await mailClient.DisconnectAsync(true);
    }
}

namespace LetsTalk.Server.Kafka.Models;

public class SendEmailRequest
{
    public string? Address { get; set; }

    public string? Subject { get; set; }

    public string? Body { get; set; }
}

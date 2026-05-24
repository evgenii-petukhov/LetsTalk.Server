namespace LetsTalk.Server.Models.Kafka;

public class SendEmailRequest
{
    public string? Address { get; set; }

    public string? Subject { get; set; }

    public string? Body { get; set; }
}

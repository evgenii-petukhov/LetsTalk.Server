namespace LetsTalk.Server.Models.Kafka;

public class IncomingCallRequest
{
    public string? CallId { get; set; }

    public string? Offer { get; set; }

    public string? ChatId { get; set; }
}

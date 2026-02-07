namespace LetsTalk.Server.Kafka.Models;

public class RtcSessionSettings
{
    public string? CallId { get; set; }

    public string? Offer { get; set; }

    public string? Answer { get; set; }

    public string? ChatId { get; set; }
}

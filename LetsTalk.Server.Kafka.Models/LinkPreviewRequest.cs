namespace LetsTalk.Server.Kafka.Models;

public class LinkPreviewRequest
{
    public string? MessageId { get; set; }

    public string[]? AccountIds { get; set; }

    public string? Url { get; set; }

    public string? ChatId { get; set; }
}
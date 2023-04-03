namespace LetsTalk.Server.Kafka.Models;

public class LinkPreviewRequest
{
    public int MessageId { get; set; }

    public string? Url { get; set; }
}

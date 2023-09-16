namespace LetsTalk.Server.Kafka.Models;

public class LinkPreviewRequest
{
    public int SenderId { get; set; }

    public int RecipientId { get; set; }

    public int MessageId { get; set; }

    public string? Url { get; set; }
}
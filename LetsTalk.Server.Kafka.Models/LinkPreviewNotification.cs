namespace LetsTalk.Server.Kafka.Models;

public class LinkPreviewNotification
{
    public int MessageId { get; set; }

    public string? Title { get; set; }

    public string? ImageUrl { get; set; }
}

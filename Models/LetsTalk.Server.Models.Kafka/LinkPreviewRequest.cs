namespace LetsTalk.Server.Models.Kafka;

public class LinkPreviewRequest
{
    public string? MessageId { get; set; }

    public string? Url { get; set; }

    public string? ChatId { get; set; }

    public string? Token { get; set; }
}
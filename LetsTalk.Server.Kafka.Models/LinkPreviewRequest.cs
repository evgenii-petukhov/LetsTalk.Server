namespace LetsTalk.Server.Kafka.Models;

public class LinkPreviewRequest
{
    public string? MessageId { get; set; }

    public List<string>? AccountIds { get; set; }

    public string? Url { get; set; }

    public string? ChatId { get; set; }

    public string? Token { get; set; }
}
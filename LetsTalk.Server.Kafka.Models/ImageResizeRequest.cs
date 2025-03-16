namespace LetsTalk.Server.Kafka.Models;

public class ImageResizeRequest
{
    public string? ImageId { get; set; }

    public string? MessageId { get; set; }

    public string[]? AccountIds { get; set; }

    public string? ChatId { get; set; }

    public int FileStorageTypeId { get; set; }
}

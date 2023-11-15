namespace LetsTalk.Server.Kafka.Models;

public class ImageResizeRequest
{
    public int ImageId { get; set; }

    public string? MessageId { get; set; }
}

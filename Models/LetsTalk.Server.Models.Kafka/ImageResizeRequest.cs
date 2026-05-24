namespace LetsTalk.Server.Models.Kafka;

public class ImageResizeRequest
{
    public string? ImageId { get; set; }

    public string? MessageId { get; set; }

    public string? ChatId { get; set; }

    public int FileStorageTypeId { get; set; }

    public string? Token {  get; set; }

    public int MaxWidth { get; set; }

    public int MaxHeight { get; set; }
}

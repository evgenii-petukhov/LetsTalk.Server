namespace LetsTalk.Server.Kafka.Models;

public class RemoveImageRequest
{
    public string? Id { get; set; }

    public int FileStorageTypeId { get; set; }
}

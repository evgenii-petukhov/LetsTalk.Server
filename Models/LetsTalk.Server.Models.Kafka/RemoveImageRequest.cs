namespace LetsTalk.Server.Models.Kafka;

public class RemoveImageRequest
{
    public string? Id { get; set; }

    public int FileStorageTypeId { get; set; }
}

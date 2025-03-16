using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.Kafka.Models;

public class RemoveImageRequest
{
    public string? ImageId { get; set; }

    public FileStorageTypes FileStorageType { get; set; }
}

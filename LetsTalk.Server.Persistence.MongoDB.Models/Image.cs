namespace LetsTalk.Server.Persistence.MongoDB.Models;

public class Image
{
    public string? Id { get; set; }

    public int ImageFormatId { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public int FileStorageTypeId { get; set; }
}

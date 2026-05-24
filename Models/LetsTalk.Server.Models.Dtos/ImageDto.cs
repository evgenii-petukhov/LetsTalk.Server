namespace LetsTalk.Server.Models.Dtos;

public record ImageDto
{
    public string? Id { get; set; }

    public int FileStorageTypeId { get; set; }
}

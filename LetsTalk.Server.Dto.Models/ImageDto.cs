namespace LetsTalk.Server.Dto.Models;

public record ImageDto
{
    public string? Id { get; set; }

    public int FileStorageTypeId { get; set; }
}

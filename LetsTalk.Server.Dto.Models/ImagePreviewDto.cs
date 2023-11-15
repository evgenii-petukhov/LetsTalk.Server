namespace LetsTalk.Server.Dto.Models;

public record ImagePreviewDto
{
    public string? MessageId { get; set; }

    public int Id { get; set; }

    public string? AccountId { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }
}

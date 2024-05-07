namespace LetsTalk.Server.Dto.Models;

public record ImagePreviewDto
{
    public string? MessageId { get; set; }

    public string? Id { get; set; }

    public string? ChatId { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }
}

namespace LetsTalk.Server.Dto.Models;

public record LinkPreviewDto
{
    public string? MessageId { get; set; }

    public string? AccountId { get; set; }

    public string? Title { get; set; }

    public string? ImageUrl { get; set; }

    public string? Url { get; set; }
}

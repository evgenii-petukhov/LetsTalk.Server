namespace LetsTalk.Server.Dto.Models;

public record LinkPreviewDto
{
    public int MessageId { get; set; }

    public int AccountId { get; set; }

    public string? Title { get; set; }

    public string? ImageUrl { get; set; }

    public string? Url { get; set; }
}

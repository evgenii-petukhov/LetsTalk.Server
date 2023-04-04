namespace LetsTalk.Server.Dto.Models;

public class LinkPreviewDto
{
    public int MessageId { get; set; }

    public int AccountId { get; set; }

    public string? Title { get; set; }

    public string? ImageUrl { get; set; }
}

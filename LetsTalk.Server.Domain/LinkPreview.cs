namespace LetsTalk.Server.Domain;

public class LinkPreview: BaseEntity
{
    public string? Url { get; set; }

    public string? Title { get; set; }

    public string? ImageUrl { get; set; }
}

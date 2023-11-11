namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

public class LinkPreviewAgnosticModel
{
    public int MessageId { get; set; }

    public int AccountId { get; set; }

    public string? Title { get; set; }

    public string? ImageUrl { get; set; }

    public string? Url { get; set; }
}

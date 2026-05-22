namespace LetsTalk.Server.Persistence.AgnosticServices.Models;

public class LinkPreviewServiceModel
{
    public int MessageId { get; set; }

    public string? Title { get; set; }

    public string? ImageUrl { get; set; }

    public string? Url { get; set; }
}

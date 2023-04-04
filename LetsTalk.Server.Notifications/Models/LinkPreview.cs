namespace LetsTalk.Server.Notifications.Models;

public class LinkPreview
{
    public int MessageId { get; set; }

    public int AccountId { get; set; }

    public string? Title { get; set; }

    public string? ImageUrl { get; set; }
}

namespace LetsTalk.Server.Configuration.Models;

public class TopicSettings
{
    public string? Notification { get; set; }

    public string? LinkPreviewRequest { get; set; }

    public string? ImageResizeRequest { get; set; }

    public string? RemoveImageRequest { get; set; }

    public string? SendLoginCodeRequest { get; set; }
}

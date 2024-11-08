namespace LetsTalk.Server.Configuration.Models;

public class TopicSettings
{
    public string? MessageNotification { get; set; }

    public string? LinkPreviewRequest { get; set; }

    public string? LinkPreviewNotification { get; set; }

    public string? ImageResizeRequest { get; set; }

    public string? SetImageDimensionsRequest { get; set; }

    public string? ImagePreviewNotification { get; set; }

    public string? RemoveImageRequest { get; set; }

    public string? SendLoginCodeRequest { get; set; }
}

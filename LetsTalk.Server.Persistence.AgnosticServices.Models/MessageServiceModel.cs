namespace LetsTalk.Server.Persistence.AgnosticServices.Models;

public class MessageServiceModel
{
    public string? Id { get; set; }

    public string? Text { get; set; }

    public string? TextHtml { get; set; }

    public string? SenderId { get; set; }

    public string? ChatId { get; set; }

    public long? DateCreatedUnix { get; set; }

    public ImageServiceModel? Image { get; set; }

    public ImagePreviewServiceModel? ImagePreview { get; set; }

    public LinkPreviewServiceModel? LinkPreview { get; set; }
}

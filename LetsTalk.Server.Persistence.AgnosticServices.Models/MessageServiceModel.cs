namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

public class MessageServiceModel
{
    public string? Id { get; set; }

    public string? Text { get; set; }

    public string? TextHtml { get; set; }

    public string? SenderId { get; set; }

    public string? ChatId { get; set; }

    public long? DateCreatedUnix { get; set; }

    public string? ImageId { get; set; }

    public ImagePreviewServiceModel? ImagePreview { get; set; }

    public LinkPreviewServiceModel? LinkPreview { get; set; }
}

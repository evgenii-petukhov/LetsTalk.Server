namespace LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;

public class MessageServiceModel
{
    public int Id { get; set; }

    public string? Text { get; set; }

    public string? TextHtml { get; set; }

    public int SenderId { get; set; }

    public int RecipientId { get; set; }

    public bool IsRead { get; set; }

    public long? DateCreatedUnix { get; set; }

    public long? DateReadUnix { get; set; }

    public int? ImageId { get; set; }

    public ImagePreviewServiceModel? ImagePreview { get; set; }

    public LinkPreviewServiceModel? LinkPreview { get; set; }
}

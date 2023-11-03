namespace LetsTalk.Server.Caching.Abstractions.Models;

public record MessageCacheEntry
{
    public int Id { get; set; }

    public string? Text { get; set; }

    public string? TextHtml { get; set; }

    public int SenderId { get; set; }

    public int RecipientId { get; set; }

    public bool? IsMine { get; set; }

    public long Created { get; set; }

    public LinkPreviewCacheEntry? LinkPreview { get; set; }

    public int? ImageId { get; set; }

    public ImagePreviewCacheEntry? ImagePreview { get; set; }
}

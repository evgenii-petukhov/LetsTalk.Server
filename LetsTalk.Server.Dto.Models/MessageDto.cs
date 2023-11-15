namespace LetsTalk.Server.Dto.Models;

public record MessageDto
{
    public string? Id { get; set; }

    public string? Text { get; set; }

    public string? TextHtml { get; set; }

    public string? SenderId { get; set; }

    public string? RecipientId { get; set; }

    public bool? IsMine { get; set; }

    public long Created { get; set; }

    public LinkPreviewDto? LinkPreview { get; set; }

    public int? ImageId { get; set; }

    public ImagePreviewDto? ImagePreview { get; set; }
}

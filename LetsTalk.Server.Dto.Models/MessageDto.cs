namespace LetsTalk.Server.Dto.Models;

public record MessageDto
{
    public string? Id { get; set; }

    public string? Text { get; set; }

    public string? TextHtml { get; set; }

    public string? ChatId { get; set; }

    public bool? IsMine { get; set; }

    public long Created { get; set; }

    public LinkPreviewDto? LinkPreview { get; set; }

    public string? ImageId { get; set; }

    public ImagePreviewDto? ImagePreview { get; set; }

    public int FileStorageTypeId { get; set; }
}

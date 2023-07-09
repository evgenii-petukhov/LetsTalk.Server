namespace LetsTalk.Server.Dto.Models;

public record MessageDto
{
    public int Id { get; set; }

    public string? Text { get; set; }

    public string? TextHtml { get; set; }

    public int SenderId { get; set; }

    public int RecipientId { get; set; }

    public bool? IsMine { get; set; }

    public long Created { get; set; }

    public LinkPreviewDto? LinkPreview { get; set; }

    public int? ImageId { get; set; }
}

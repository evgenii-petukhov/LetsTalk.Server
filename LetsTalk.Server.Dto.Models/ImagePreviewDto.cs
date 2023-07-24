namespace LetsTalk.Server.Dto.Models;

public class ImagePreviewDto
{
    public int MessageId { get; set; }

    public int Id { get; set; }

    public int AccountId { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }
}

namespace LetsTalk.Server.ImageProcessor.Models;

public class ImageResizeRequest
{
    public int ImageId { get; set; }

    public int MessageId { get; set; }

    public int RecipientId { get; set; }

    public int SenderId { get; set; }
}

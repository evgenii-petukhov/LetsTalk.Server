namespace LetsTalk.Server.ImageProcessor.Models;

public class ImageResizeRequest
{
    public int ImageId { get; set; }

    public int MaxWidth { get; set; }

    public int MaxHeight { get; set; }
}

namespace LetsTalk.Server.ImageProcessing.Abstractions.Models;

public class ImageResizeResult
{
    public byte[]? Data { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }
}

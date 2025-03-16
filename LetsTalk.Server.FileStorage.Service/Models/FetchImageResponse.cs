namespace LetsTalk.Server.FileStorage.Service.Models;

public class FetchImageResponse
{
    public byte[]? Content { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }
}
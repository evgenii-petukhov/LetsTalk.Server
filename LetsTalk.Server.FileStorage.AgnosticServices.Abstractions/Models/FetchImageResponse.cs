namespace LetsTalk.Server.FileStorage.AgnosticServices.Abstractions.Models;

public class FetchImageResponse
{
    public byte[]? Content { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }
}
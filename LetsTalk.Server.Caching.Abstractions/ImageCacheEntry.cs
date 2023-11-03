namespace LetsTalk.Server.Caching.Abstractions;

public class ImageCacheEntry
{
    public byte[]? Content { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }
}

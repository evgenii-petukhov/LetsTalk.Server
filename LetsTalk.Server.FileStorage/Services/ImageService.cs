using LetsTalk.Server.FileStorage.Abstractions;
using LetsTalk.Server.FileStorage.Models;
using SkiaSharp;

namespace LetsTalk.Server.FileStorage.Services;

public class ImageService : IImageService
{
    public ImageInfo GetImageInfo(byte[] data)
    {
        using var dataStream = new MemoryStream(data);
        using var skiaStream = new SKManagedStream(dataStream);
        var bitmap = SKBitmap.DecodeBounds(skiaStream);
        return new ImageInfo
        {
            Width = bitmap.Width,
            Height = bitmap.Height
        };
    }
}

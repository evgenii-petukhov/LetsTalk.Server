using LetsTalk.Server.ImageProcessing.ImageResizeEngine.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using SkiaSharp;

namespace LetsTalk.Server.ImageProcessing.ImageResizeEngine;

public class ImageInfoService : IImageInfoService
{
    private readonly Dictionary<SKEncodedImageFormat, ImageFormats> _formatMapping = new()
    {
        { SKEncodedImageFormat.Jpeg, ImageFormats.Jpeg },
        { SKEncodedImageFormat.Png, ImageFormats.Png },
        { SKEncodedImageFormat.Gif, ImageFormats.Gif },
        { SKEncodedImageFormat.Webp, ImageFormats.Webp },
    };

    public (int, int) GetImageSize(byte[] data)
    {
        using var stream = new MemoryStream(data);
        using var skiaStream = new SKManagedStream(stream);
        var bitmap = SKBitmap.DecodeBounds(skiaStream);
        return (bitmap.Width, bitmap.Height);
    }

    public ImageFormats GetImageFormat(byte[] data)
    {
        using var stream = new MemoryStream(data);
        using var skiaStream = new SKManagedStream(stream);
        using var codec = SKCodec.Create(skiaStream);
        return _formatMapping.GetValueOrDefault(codec.EncodedFormat, ImageFormats.Unknown);
    }
}
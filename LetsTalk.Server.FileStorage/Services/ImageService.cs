using LetsTalk.Server.FileStorage.Abstractions;
using LetsTalk.Server.FileStorage.Models;
using LetsTalk.Server.Persistence.Models;
using SkiaSharp;

namespace LetsTalk.Server.FileStorage.Services;

public class ImageService : IImageService
{
    private readonly Dictionary<SKEncodedImageFormat, ImageContentTypes> _contentTypeMapping = new()
    {
        { SKEncodedImageFormat.Jpeg, ImageContentTypes.Jpeg },
        { SKEncodedImageFormat.Png, ImageContentTypes.Png },
        { SKEncodedImageFormat.Gif, ImageContentTypes.Gif },
        { SKEncodedImageFormat.Webp, ImageContentTypes.Webp },
    };

    public ImageSize GetImageSize(byte[] data)
    {
        using var stream = new MemoryStream(data);
        using var skiaStream = new SKManagedStream(stream);
        var bitmap = SKBitmap.DecodeBounds(skiaStream);
        return new ImageSize
        {
            Width = bitmap.Width,
            Height = bitmap.Height
        };
    }

    public ImageContentTypes GetImageContentType(byte[] data)
    {
        using var stream = new MemoryStream(data);
        using var skiaStream = new SKManagedStream(stream);
        using var codec = SKCodec.Create(skiaStream);
        return _contentTypeMapping.GetValueOrDefault(codec.EncodedFormat, ImageContentTypes.Unknown);
    }
}

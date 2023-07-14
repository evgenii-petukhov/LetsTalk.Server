using LetsTalk.Server.ImageProcessing.Abstractions;
using SkiaSharp;

namespace LetsTalk.Server.ImageProcessing;

public class ImageResizeService : IImageResizeService
{
    public byte[] Resize(byte[] data, int maxWidth, int maxHeight)
    {
        using var ms = new MemoryStream(data);
        using var sourceBitmap = SKBitmap.Decode(ms);

        var scaleX = sourceBitmap.Width > maxWidth ? (double)maxWidth / sourceBitmap.Width : 1d;
        var scaleY = sourceBitmap.Height > maxHeight ? (double)maxHeight / sourceBitmap.Height : 1d;
        var scale = Math.Min(scaleX, scaleY);

        using var scaledBitmap = sourceBitmap.Resize(new SKImageInfo((int)(scale * sourceBitmap.Width), (int)(scale * sourceBitmap.Height)), SKFilterQuality.High);
        using var scaledImage = SKImage.FromBitmap(scaledBitmap);
        using var scaledData = scaledImage.Encode(SKEncodedImageFormat.Webp, 92);

        return scaledData.ToArray();
    }
}

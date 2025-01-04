using LetsTalk.Server.ImageProcessing.Abstractions;
using LetsTalk.Server.ImageProcessing.Abstractions.Models;
using SkiaSharp;

namespace LetsTalk.Server.ImageProcessing.Utility;

public class ImageResizeService : IImageResizeService
{
    public ImageResizeResult Resize(byte[] data, int maxWidth, int maxHeight)
    {
        using var ms = new MemoryStream(data);
        using var sourceBitmap = SKBitmap.Decode(ms);

        var scaleX = sourceBitmap.Width > maxWidth ? (double)maxWidth / sourceBitmap.Width : 1d;
        var scaleY = sourceBitmap.Height > maxHeight ? (double)maxHeight / sourceBitmap.Height : 1d;
        var scale = Math.Min(scaleX, scaleY);

        var targetWidth = (int)(scale * sourceBitmap.Width);
        var targetHeight = (int)(scale * sourceBitmap.Height);

        using var scaledBitmap = sourceBitmap.Resize(new SKImageInfo(targetWidth, targetHeight), SKSamplingOptions.Default);
        using var scaledImage = SKImage.FromBitmap(scaledBitmap);
        using var scaledData = scaledImage.Encode(SKEncodedImageFormat.Webp, 92);

        return new ImageResizeResult
        {
            Data = scaledData.ToArray(),
            Width = targetWidth,
            Height = targetHeight
        };
    }
}

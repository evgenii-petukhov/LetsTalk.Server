using KafkaFlow;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.ImageProcessor.Models;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.Extensions.Options;
using SkiaSharp;

namespace LetsTalk.Server.ImageProcessor;

public class ImageResizeRequestHandler : IMessageHandler<ImageResizeRequest>
{
    private readonly IImageService _imageService;
    private readonly FileStorageSettings _fileStorageSettings;

    public ImageResizeRequestHandler(
        IImageService imageService,
        IOptions<FileStorageSettings> options)
    {
        _imageService = imageService;
        _fileStorageSettings = options.Value;
    }

    public async Task Handle(IMessageContext context, ImageResizeRequest message)
    {
        var data = await _imageService.FetchImageAsync(message.ImageId);
        var scaledData = Resize(data, _fileStorageSettings.ImagePreviewMaxWidth, _fileStorageSettings.ImagePreviewMaxHeight);
        var imageId = await _imageService.SaveImageAsync(scaledData, ImageRoles.Message, ImageFormats.Webp);
    }

    private static byte[] Resize(byte[] data, int maxWidth, int maxHeight)
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

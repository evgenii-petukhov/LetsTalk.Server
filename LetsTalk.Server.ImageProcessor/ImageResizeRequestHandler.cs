using KafkaFlow;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.ImageProcessing.Abstractions;
using LetsTalk.Server.ImageProcessor.Models;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.ImageProcessor;

public class ImageResizeRequestHandler : IMessageHandler<ImageResizeRequest>
{
    private readonly IImageService _imageService;
    private readonly IImageResizeService _imageResizeService;
    private readonly FileStorageSettings _fileStorageSettings;

    public ImageResizeRequestHandler(
        IImageService imageService,
        IImageResizeService imageResizeService,
        IOptions<FileStorageSettings> options)
    {
        _imageService = imageService;
        _imageResizeService = imageResizeService;
        _fileStorageSettings = options.Value;
    }

    public async Task Handle(IMessageContext context, ImageResizeRequest message)
    {
        var data = await _imageService.FetchImageAsync(message.ImageId);
        var scaledData = _imageResizeService.Resize(data, _fileStorageSettings.ImagePreviewMaxWidth, _fileStorageSettings.ImagePreviewMaxHeight);
        var imageId = await _imageService.SaveImageAsync(scaledData, ImageRoles.Message, ImageFormats.Webp);
    }
}

using KafkaFlow;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.ImageProcessing.Abstractions;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.ImageProcessing.Service;

public class ImageResizeRequestHandler : IMessageHandler<ImageResizeRequest>
{
    private readonly IImageService _imageService;
    private readonly IFileService _fileService;
    private readonly IImageResizeService _imageResizeService;
    private readonly IImageDataLayerService _imageDataLayerService;
    private readonly FileStorageSettings _fileStorageSettings;

    public ImageResizeRequestHandler(
        IImageService imageService,
        IFileService fileService,
        IImageResizeService imageResizeService,
        IImageDataLayerService imageDataLayerService,
        IOptions<FileStorageSettings> fileStorageSettings)
    {
        _imageService = imageService;
        _fileService = fileService;
        _imageResizeService = imageResizeService;
        _imageDataLayerService = imageDataLayerService;
        _fileStorageSettings = fileStorageSettings.Value;
    }

    public async Task Handle(IMessageContext context, ImageResizeRequest message)
    {
        var fetchImageResponse = await _imageService.FetchImageAsync(message.ImageId);
        var (data, width, height) = _imageResizeService.Resize(
            fetchImageResponse.Content!,
            _fileStorageSettings.ImagePreviewMaxWidth,
            _fileStorageSettings.ImagePreviewMaxHeight);
        var filename = await _fileService.SaveDataAsync(data!, FileTypes.Image, ImageRoles.Message);

        await _imageDataLayerService.CreateImagePreviewAsync(
            filename,
            ImageFormats.Webp,
            width,
            height,
            message.MessageId);
    }
}

using KafkaFlow;
using KafkaFlow.Producers;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.ImageProcessing.Abstractions;
using LetsTalk.Server.ImageProcessor.Models;
using LetsTalk.Server.Notifications.Models;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.ImageProcessing.Service;

public class ImageResizeRequestHandler : IMessageHandler<ImageResizeRequest>
{
    private readonly IImageService _imageService;
    private readonly IFileService _fileService;
    private readonly IImageResizeService _imageResizeService;
    private readonly IImageDataLayerService _imageDataLayerService;
    private readonly FileStorageSettings _fileStorageSettings;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IMessageProducer _producer;

    public ImageResizeRequestHandler(
        IImageService imageService,
        IFileService fileService,
        IImageResizeService imageResizeService,
        IImageDataLayerService imageDataLayerService,
        IOptions<FileStorageSettings> fileStorageSettings,
        IOptions<KafkaSettings> kafkaSettings,
        IProducerAccessor producerAccessor)
    {
        _imageService = imageService;
        _fileService = fileService;
        _imageResizeService = imageResizeService;
        _imageDataLayerService = imageDataLayerService;
        _fileStorageSettings = fileStorageSettings.Value;
        _kafkaSettings = kafkaSettings.Value;
        _producer = producerAccessor.GetProducer(_kafkaSettings.ImagePreviewNotification!.Producer);
    }

    public async Task Handle(IMessageContext context, ImageResizeRequest message)
    {
        var fetchImageResponse = await _imageService.FetchImageAsync(message.ImageId);
        var resizeResult = _imageResizeService.Resize(
            fetchImageResponse.Content!,
            _fileStorageSettings.ImagePreviewMaxWidth,
            _fileStorageSettings.ImagePreviewMaxHeight);
        var filename = await _fileService.SaveDataAsync(fetchImageResponse.Content!.ToArray(), FileTypes.Image, ImageRoles.Message);

        var image = await _imageDataLayerService.CreateImagePreviewAsync(filename, ImageFormats.Webp, resizeResult.Width, resizeResult.Height, message.MessageId);

        var imagePreviewDto = new ImagePreviewDto
        {
            MessageId = message.MessageId,
            Id = image.Id,
        };

        await _producer.ProduceAsync(
            _kafkaSettings.ImagePreviewNotification!.Topic,
            Guid.NewGuid().ToString(),
            new Notification<ImagePreviewDto>[]
            {
                new Notification<ImagePreviewDto>
                {
                    RecipientId = message.RecipientId,
                    Message = imagePreviewDto with
                    {
                        AccountId = message.SenderId
                    }
                },
                new Notification<ImagePreviewDto>
                {
                    RecipientId = message.SenderId,
                    Message = imagePreviewDto with
                    {
                        AccountId = message.RecipientId
                    }
                }
            });
    }
}

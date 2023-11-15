using KafkaFlow;
using KafkaFlow.Producers;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.ImageProcessing.Abstractions;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Notifications.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.ImageProcessing.Service;

public class ImageResizeRequestHandler : IMessageHandler<ImageResizeRequest>
{
    private readonly IImageService _imageService;
    private readonly IFileService _fileService;
    private readonly IImageResizeService _imageResizeService;

    private readonly IImageAgnosticService _imageAgnosticService;
    private readonly IMessageProducer _producer;
    private readonly KafkaSettings _kafkaSettings;
    private readonly FileStorageSettings _fileStorageSettings;

    public ImageResizeRequestHandler(
        IImageService imageService,
        IFileService fileService,
        IImageResizeService imageResizeService,
        IImageAgnosticService imageAgnosticService,
        IOptions<KafkaSettings> kafkaSettings,
        IOptions<FileStorageSettings> fileStorageSettings,
        IProducerAccessor producerAccessor)
    {
        _imageService = imageService;
        _fileService = fileService;
        _imageResizeService = imageResizeService;
        _imageAgnosticService = imageAgnosticService;
        _kafkaSettings = kafkaSettings.Value;
        _fileStorageSettings = fileStorageSettings.Value;
        _producer = producerAccessor.GetProducer(_kafkaSettings.ImagePreviewNotification!.Producer);
    }

    public async Task Handle(IMessageContext context, ImageResizeRequest request)
    {
        var fetchImageResponse = await _imageService.FetchImageAsync(request.ImageId);
        var (data, width, height) = _imageResizeService.Resize(
            fetchImageResponse.Content!,
            _fileStorageSettings.ImagePreviewMaxWidth,
            _fileStorageSettings.ImagePreviewMaxHeight);
        var filename = await _fileService.SaveDataAsync(data!, FileTypes.Image, ImageRoles.Message);

        var message = await _imageAgnosticService.SaveImagePreviewAsync(
            request.MessageId!,
            filename,
            ImageFormats.Webp,
            ImageRoles.Message,
            width,
            height);

        var imagePreviewDto = new ImagePreviewDto
        {
            MessageId = message!.Id,
            Id = message.ImagePreview!.Id,
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

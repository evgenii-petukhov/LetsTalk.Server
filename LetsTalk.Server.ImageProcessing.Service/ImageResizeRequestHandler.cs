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
    private readonly IImageResizeService _imageResizeService;
    private readonly IMessageRepository _messageRepository;
    private readonly FileStorageSettings _fileStorageSettings;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IMessageProducer _producer;

    public ImageResizeRequestHandler(
        IImageService imageService,
        IImageResizeService imageResizeService,
        IMessageRepository messageRepository,
        IProducerAccessor producerAccessor,
        IOptions<FileStorageSettings> fileStorageSettings,
        IOptions<KafkaSettings> kafkaSettings)
    {
        _imageService = imageService;
        _imageResizeService = imageResizeService;
        _messageRepository = messageRepository;
        _fileStorageSettings = fileStorageSettings.Value;
        _kafkaSettings = kafkaSettings.Value;
        _producer = producerAccessor.GetProducer(_kafkaSettings.ImagePreviewNotification!.Producer);
    }

    public async Task Handle(IMessageContext context, ImageResizeRequest message)
    {
        var data = await _imageService.FetchImageAsync(message.ImageId);
        var resizeResult = _imageResizeService.Resize(
            data,
            _fileStorageSettings.ImagePreviewMaxWidth,
            _fileStorageSettings.ImagePreviewMaxHeight);
        var scaledImageId = await _imageService.SaveImageAsync(
            resizeResult.Data!,
            ImageRoles.Message,
            ImageFormats.Webp,
            resizeResult.Width,
            resizeResult.Height);
        await _messageRepository.SetImagePreviewAsync(message.MessageId, scaledImageId);
        await Task.WhenAll(
            SendNotificationAsync(message.SenderId, message.RecipientId, message.MessageId, scaledImageId),
            SendNotificationAsync(message.RecipientId, message.SenderId, message.MessageId, scaledImageId));
    }

    private Task SendNotificationAsync(int senderId, int recipientId, int messageId, int imageId)
    {
        return _producer.ProduceAsync(
            _kafkaSettings.ImagePreviewNotification!.Topic,
            Guid.NewGuid().ToString(),
            new Notification<ImagePreviewDto>
            {
                RecipientId = recipientId,
                Message = new ImagePreviewDto
                {
                    MessageId = messageId,
                    ImagePreviewId = imageId,
                    AccountId = senderId
                }
            });
    }
}

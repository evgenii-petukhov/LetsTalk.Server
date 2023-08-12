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
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.ImageProcessing.Service;

public class ImageResizeRequestHandler : IMessageHandler<ImageResizeRequest>
{
    private readonly IImageService _imageService;
    private readonly IImageResizeService _imageResizeService;
    private readonly IMessageRepository _messageRepository;
    private readonly LetsTalkDbContext _context;
    private readonly FileStorageSettings _fileStorageSettings;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IMessageProducer _producer;

    public ImageResizeRequestHandler(
        IImageService imageService,
        IImageResizeService imageResizeService,
        IMessageRepository messageRepository,
        IProducerAccessor producerAccessor,
        IOptions<FileStorageSettings> fileStorageSettings,
        IOptions<KafkaSettings> kafkaSettings,
        LetsTalkDbContext context)
    {
        _imageService = imageService;
        _imageResizeService = imageResizeService;
        _messageRepository = messageRepository;
        _context = context;
        _fileStorageSettings = fileStorageSettings.Value;
        _kafkaSettings = kafkaSettings.Value;
        _producer = producerAccessor.GetProducer(_kafkaSettings.ImagePreviewNotification!.Producer);
    }

    public async Task Handle(IMessageContext context, ImageResizeRequest message)
    {
        var data = await _imageService.FetchImageAsync(message.ImageId);
        var resizeResult = _imageResizeService.Resize(
            data.Content!,
            _fileStorageSettings.ImagePreviewMaxWidth,
            _fileStorageSettings.ImagePreviewMaxHeight);

        await using var transaction = await _context.Database.BeginTransactionAsync();

        var imageId = await _imageService.SaveImageAsync(
            resizeResult.Data!,
            ImageRoles.Message,
            ImageFormats.Webp,
            resizeResult.Width,
            resizeResult.Height);

        await _messageRepository.SetImagePreviewAsync(message.MessageId, imageId);

        await transaction.CommitAsync();

        var imagePreviewDto = new ImagePreviewDto
        {
            MessageId = message.MessageId,
            Id = imageId,
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

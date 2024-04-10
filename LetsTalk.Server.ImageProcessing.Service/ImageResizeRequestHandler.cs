using AutoMapper;
using KafkaFlow;
using KafkaFlow.Producers;
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

    private readonly IMessageAgnosticService _messageAgnosticService;
    private readonly IMapper _mapper;
    private readonly IMessageProducer _producer;
    private readonly KafkaSettings _kafkaSettings;
    private readonly FileStorageSettings _fileStorageSettings;

    public ImageResizeRequestHandler(
        IImageService imageService,
        IFileService fileService,
        IImageResizeService imageResizeService,
        IMessageAgnosticService messageAgnosticService,
        IMapper mapper,
        IOptions<KafkaSettings> kafkaSettings,
        IOptions<FileStorageSettings> fileStorageSettings,
        IProducerAccessor producerAccessor)
    {
        _imageService = imageService;
        _fileService = fileService;
        _imageResizeService = imageResizeService;
        _messageAgnosticService = messageAgnosticService;
        _mapper = mapper;
        _kafkaSettings = kafkaSettings.Value;
        _fileStorageSettings = fileStorageSettings.Value;
        _producer = producerAccessor.GetProducer(_kafkaSettings.ImagePreviewNotification!.Producer);
    }

    public async Task Handle(IMessageContext context, ImageResizeRequest request)
    {
        var fetchImageResponse = await _imageService.FetchImageAsync(request.ImageId!);

        if (fetchImageResponse == null)
        {
            return;
        }

        var (data, width, height) = _imageResizeService.Resize(
            fetchImageResponse.Content!,
            _fileStorageSettings.ImagePreviewMaxWidth,
            _fileStorageSettings.ImagePreviewMaxHeight);

        var filename = await _fileService.SaveDataAsync(data!, FileTypes.Image, width, height);
        await _fileService.SaveImageInfoAsync(filename, width, height);

        var message = await _messageAgnosticService.SaveImagePreviewAsync(
            request.MessageId!,
            filename,
            ImageFormats.Webp,
            width,
            height);

        var imagePreviewDto = _mapper.Map<ImagePreviewDto>(message);

        await _producer.ProduceAsync(
            _kafkaSettings.ImagePreviewNotification!.Topic,
            Guid.NewGuid().ToString(),
            request.AccountIds!.Select(accountId => new Notification<ImagePreviewDto>
            {
                RecipientId = accountId,
                Message = imagePreviewDto
            }).ToArray());
    }
}

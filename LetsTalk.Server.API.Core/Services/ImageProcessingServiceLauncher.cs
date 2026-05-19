using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Kafka.Models;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.API.Core.Services;

public class ImageProcessingServiceLauncher(
    IProducer<ImageResizeRequest> imageResizeProducer,
    IOptions<ImageConstraints> options) : IImageProcessingLauncher
{
    private readonly IProducer<ImageResizeRequest> _imageResizeProducer = imageResizeProducer;
    private readonly ImageConstraints _imageConstraints = options.Value;

    public Task LaunchAsync(
        string messageId,
        string imageId,
        string chatId,
        int fileStorageTypeId,
        string token,
        CancellationToken cancellationToken = default)
    {
        return _imageResizeProducer.PublishAsync(new ImageResizeRequest
        {
            MessageId = messageId,
            ImageId = imageId,
            ChatId = chatId,
            FileStorageTypeId = fileStorageTypeId,
            Token = token,
            MaxWidth = _imageConstraints.ImagePreviewMaxWidth,
            MaxHeight = _imageConstraints.ImagePreviewMaxHeight
        }, cancellationToken);
    }
}

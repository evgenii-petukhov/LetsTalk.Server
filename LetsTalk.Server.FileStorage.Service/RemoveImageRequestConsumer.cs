using LetsTalk.Server.FileStorage.AgnosticServices.Abstractions;
using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.Enums;
using MassTransit;

namespace LetsTalk.Server.FileStorage.Service;

public class RemoveImageRequestConsumer(
    IAgnosticFileService fileService,
    IImageCacheManager imageCacheManager) : IConsumer<RemoveImageRequest>
{
    private readonly IAgnosticFileService _fileService = fileService;
    private readonly IImageCacheManager _imageCacheManager = imageCacheManager;

    public Task Consume(ConsumeContext<RemoveImageRequest> context)
    {
        _fileService.DeleteFile(context.Message.ImageId!, FileTypes.Image);
        _fileService.DeleteFile(context.Message.ImageId! + ".info", FileTypes.Image);

        return _imageCacheManager.ClearAsync(context.Message.ImageId!);
    }
}

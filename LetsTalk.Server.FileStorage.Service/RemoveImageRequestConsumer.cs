using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.Enums;
using MassTransit;

namespace LetsTalk.Server.FileStorage.Service;

public class RemoveImageRequestConsumer(
    IIOService ioService,
    IImageCacheManager imageCacheManager) : IConsumer<RemoveImageRequest>
{
    private readonly IIOService _ioService = ioService;
    private readonly IImageCacheManager _imageCacheManager = imageCacheManager;

    public Task Consume(ConsumeContext<RemoveImageRequest> context)
    {
        _ioService.DeleteFile(context.Message.ImageId!, FileTypes.Image);
        _ioService.DeleteFile(context.Message.ImageId! + ".info", FileTypes.Image);

        return _imageCacheManager.ClearAsync(context.Message.ImageId!);
    }
}

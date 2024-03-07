using KafkaFlow;
using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Service;

public class RemoveImageRequestHandler(
    IIOService iOService,
    IImageCacheManager imageCacheManager) : IMessageHandler<RemoveImageRequest>
{
    private readonly IIOService _iOService = iOService;
    private readonly IImageCacheManager _imageCacheManager = imageCacheManager;

    public Task Handle(IMessageContext context, RemoveImageRequest message)
    {
        _iOService.DeleteFile(message.ImageId!, FileTypes.Image);
        _iOService.DeleteFile(message.ImageId! + ".info", FileTypes.Image);

        return _imageCacheManager.RemoveAsync(message.ImageId!);
    }
}

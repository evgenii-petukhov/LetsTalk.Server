using KafkaFlow;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Service;

public class RemoveImageRequestHandler : IMessageHandler<RemoveImageRequest>
{
    private readonly IIOService _iOService;
    private readonly IImageCacheManager _imageCacheManager;
    private readonly IImageAgnosticService _imageAgnosticService;
    private readonly IFileAgnosticService _fileAgnosticService;

    public RemoveImageRequestHandler(
        IIOService iOService,
        IImageCacheManager imageCacheManager,
        IImageAgnosticService imageAgnosticService,
        IFileAgnosticService fileAgnosticService)
    {
        _iOService = iOService;
        _imageCacheManager = imageCacheManager;
        _imageAgnosticService = imageAgnosticService;
        _fileAgnosticService = fileAgnosticService;
    }

    public async Task Handle(IMessageContext context, RemoveImageRequest message)
    {
        var image = await _imageAgnosticService.GetByIdWithFileAsync(message.ImageId!);
        await _fileAgnosticService.DeleteByIdAsync(image!.File!.Id);

        _iOService.DeleteFile(image.File!.FileName!, FileTypes.Image);

        await _imageCacheManager.RemoveAsync(message.ImageId!);
    }
}

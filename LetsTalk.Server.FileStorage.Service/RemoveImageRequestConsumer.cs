using LetsTalk.Server.FileStorage.Abstractions;
using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.Enums;
using MassTransit;

namespace LetsTalk.Server.FileStorage.Service;

public class RemoveImageRequestConsumer(
    IFileServiceResolver fileServiceResolver,
    IImageStorageCacheManager imageStorageCacheManager) : IConsumer<RemoveImageRequest>
{
    private readonly IImageStorageCacheManager _imageStorageCacheManager = imageStorageCacheManager;
    private readonly IFileServiceResolver _fileServiceResolver = fileServiceResolver;

    public async Task Consume(ConsumeContext<RemoveImageRequest> context)
    {
        var fileService = _fileServiceResolver.Resolve(context.Message.FileStorageType);
        await fileService.DeleteFileAsync(context.Message.ImageId!, FileTypes.Image);
        await fileService.DeleteFileAsync(context.Message.ImageId! + ".info", FileTypes.Image);
        await _imageStorageCacheManager.ClearAsync(context.Message.ImageId!);
    }
}

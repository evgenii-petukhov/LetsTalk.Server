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

    public Task Consume(ConsumeContext<RemoveImageRequest> context)
    {
        var fileService = _fileServiceResolver.Resolve(context.Message.FileStorageType);
        fileService.DeleteFile(context.Message.ImageId!, FileTypes.Image);
        fileService.DeleteFile(context.Message.ImageId! + ".info", FileTypes.Image);

        return _imageStorageCacheManager.ClearAsync(context.Message.ImageId!);
    }
}

using KafkaFlow;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.FileStorage.Service;

public class RemoveImageRequestHandler : IMessageHandler<RemoveImageRequest>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileRepository _fileRepository;
    private readonly IImageRepository _imageRepository;
    private readonly IIOService _iOService;
    private readonly IImageCacheManager _imageCacheManager;

    public RemoveImageRequestHandler(
        IUnitOfWork unitOfWork,
        IFileRepository fileRepository,
        IImageRepository imageRepository,
        IIOService iOService,
        IImageCacheManager imageCacheManager)
    {
        _unitOfWork = unitOfWork;
        _fileRepository = fileRepository;
        _imageRepository = imageRepository;
        _iOService = iOService;
        _imageCacheManager = imageCacheManager;
    }

    public async Task Handle(IMessageContext context, RemoveImageRequest message)
    {
        var file = await _imageRepository.GetByIdWithFileAsync(message.ImageId, x => x.File);
        _fileRepository.Delete(file!);
        await _unitOfWork.SaveAsync();

        _iOService.DeleteFile(file!.FileName!, FileTypes.Image);

        await _imageCacheManager.RemoveAsync(message.ImageId);
    }
}

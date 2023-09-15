using KafkaFlow;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.FileStorage.Models;
using LetsTalk.Server.FileStorage.Service.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.FileStorage.Service;

public class RemoveImageRequestHandler : IMessageHandler<RemoveImageRequest>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEntityFactory _entityFactory;
    private readonly IFileRepository _fileRepository;
    private readonly IImageRepository _imageRepository;
    private readonly IIOService _iOService;

    public RemoveImageRequestHandler(
        IUnitOfWork unitOfWork,
        IEntityFactory entityFactory,
        IFileRepository fileRepository,
        IImageRepository imageRepository,
        IIOService iOService)
    {
        _unitOfWork = unitOfWork;
        _entityFactory = entityFactory;
        _fileRepository = fileRepository;
        _imageRepository = imageRepository;
        _iOService = iOService;
    }

    public async Task Handle(IMessageContext context, RemoveImageRequest message)
    {
        var fileInfo = await _imageRepository.GetByIdWithFileAsync(message.ImageId, x => new
        {
            Id = x.FileId,
            x.File!.FileName
        });
        var file = _entityFactory.CreateFile(fileInfo!.Id);
        _fileRepository.Delete(file);
        await _unitOfWork.SaveAsync();

        _iOService.DeleteFile(fileInfo.FileName!, FileTypes.Image);
    }
}

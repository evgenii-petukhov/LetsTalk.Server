using KafkaFlow;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.ImageProcessing.Abstractions;
using LetsTalk.Server.ImageProcessor.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.ImageProcessing.Service;

public class SetImageDimensionsRequestHandler : IMessageHandler<SetImageDimensionsRequest>
{
    private readonly IFileService _fileService;
    private readonly IImageInfoService _imageInfoService;
    private readonly IImageRepository _imageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetImageDimensionsRequestHandler(
        IFileService fileService,
        IImageInfoService imageInfoService,
        IImageRepository imageRepository,
        IUnitOfWork unitOfWork)
    {
        _fileService = fileService;
        _imageInfoService = imageInfoService;
        _imageRepository = imageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(IMessageContext context, SetImageDimensionsRequest message)
    {
        var image = await _imageRepository.GetByIdAsTrackingAsync(message.ImageId);
        var data = await _fileService.ReadFileAsync(image!.File!.FileName!, FileTypes.Image);
        var (width, height) = _imageInfoService.GetImageSize(data);
        image.SetDimensions(width, height);
        await _unitOfWork.SaveAsync();
    }
}

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

    public SetImageDimensionsRequestHandler(
        IFileService fileService,
        IImageInfoService imageInfoService,
        IImageRepository imageRepository)
    {
        _fileService = fileService;
        _imageInfoService = imageInfoService;
        _imageRepository = imageRepository;
    }

    public async Task Handle(IMessageContext context, SetImageDimensionsRequest message)
    {
        var filename = await _imageRepository.GetByIdOrDefaultAsync(message.ImageId, x => x.File!.FileName);
        var data = await _fileService.ReadFileAsync(filename!, FileTypes.Image);
        var (width, height) = _imageInfoService.GetImageSize(data);
        await _imageRepository.SetDimensionsAsync(message.ImageId, width, height);
    }
}

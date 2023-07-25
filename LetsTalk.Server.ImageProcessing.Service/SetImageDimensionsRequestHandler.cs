using KafkaFlow;
using KafkaFlow.TypedHandler;
using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.ImageProcessing.Abstractions;
using LetsTalk.Server.ImageProcessor.Models;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.ImageProcessing.Service;

public class SetImageDimensionsRequestHandler : IMessageHandler<SetImageDimensionsRequest>
{
    private static readonly SemaphoreSlim SemaphoreSlim = new(1, 1);

    private readonly IImageDataLayerService _imageDataLayerService;
    private readonly IFileService _fileService;
    private readonly IImageInfoService _imageInfoService;
    private readonly IImageRepository _imageRepository;

    public SetImageDimensionsRequestHandler(
        IImageDataLayerService imageDataLayerService,
        IFileService fileService,
        IImageInfoService imageInfoService,
        IImageRepository imageRepository)
    {
        _imageDataLayerService = imageDataLayerService;
        _fileService = fileService;
        _imageInfoService = imageInfoService;
        _imageRepository = imageRepository;
    }

    public async Task Handle(IMessageContext context, SetImageDimensionsRequest message)
    {
        await SemaphoreSlim.WaitAsync();
        try
        {
            var filename = await _imageDataLayerService.GetByIdOrDefaultAsync(message.ImageId, x => x.File!.FileName);
            var data = await _fileService.ReadFileAsync(filename!, FileTypes.Image);
            var (width, height) = _imageInfoService.GetImageSize(data);
            await _imageRepository.SetDimensionsAsync(message.ImageId, width, height);
        }
        finally
        {
            SemaphoreSlim.Release();
        }

    }
}

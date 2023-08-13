using LetsTalk.Server.FileStorage.Models;
using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Utility;

public class ImageService : IImageService
{
    private readonly IFileService _fileService;
    private readonly IImageDataLayerService _imageDataLayerService;

    public ImageService(
        IFileService fileService,
        IImageDataLayerService imageDataLayerService)
    {
        _fileService = fileService;
        _imageDataLayerService = imageDataLayerService;
    }

    public async Task<FetchImageResponse> FetchImageAsync(int imageId, bool useDimensions = false, CancellationToken cancellationToken = default)
    {
        dynamic? image = useDimensions
            ? await _imageDataLayerService.GetByIdOrDefaultAsync(imageId, x => new
            {
                x.File!.FileName,
                x.Width,
                x.Height
            }, cancellationToken)
            : await _imageDataLayerService.GetByIdOrDefaultAsync(imageId, x => new
            {
                x.File!.FileName,
                Width = 0,
                Height = 0
            }, cancellationToken);

        return new FetchImageResponse
        {
            Content = await _fileService.ReadFileAsync(image!.FileName!, FileTypes.Image, cancellationToken),
            Width = image.Width ?? 0,
            Height = image.Height ?? 0
        };
    }
}

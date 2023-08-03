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

    public async Task<int> SaveImageAsync(
        byte[] content,
        ImageRoles imageRole,
        ImageFormats imageFormat,
        int width,
        int height,
        CancellationToken cancellationToken = default)
    {
        var data = content.ToArray();
        var filename = await _fileService.SaveDataAsync(data, FileTypes.Image, imageRole, cancellationToken);
        return await _imageDataLayerService.CreateWithFileAsync(filename, imageFormat, imageRole, width, height, cancellationToken);
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

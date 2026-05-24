using LetsTalk.Server.FileStorage.Abstractions;
using LetsTalk.Server.ImageProcessing.ImageResizeEngine.Abstractions;
using LetsTalk.Server.ImageProcessing.Utility.Abstractions;
using LetsTalk.Server.ImageProcessing.Utility.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.ImageProcessing.Utility;

public class ImageProcessingService(
    IFileServiceResolver fileServiceResolver,
    IImageResizeService imageResizeService) : IImageProcessingService
{
    private readonly IImageResizeService _imageResizeService = imageResizeService;

    public async Task<ProcessImageResponse> ProcessImageAsync(
        string imageId,
        int maxWidth,
        int maxHeight,
        FileStorageTypes fileStorageTypeId,
        CancellationToken cancellationToken = default)
    {
        var fileService = fileServiceResolver.Resolve(fileStorageTypeId);

        var content = await fileService.ReadFileAsync(imageId, FileTypes.Image, cancellationToken);

        var (data, width, height) = _imageResizeService.Resize(
            content,
            maxWidth,
            maxHeight);

        var filename = await fileService.SaveDataAsync(data!, FileTypes.Image, width, height, cancellationToken: cancellationToken);
        await fileService.SaveImageInfoAsync(filename, width, height, cancellationToken);

        return new ProcessImageResponse
        {
            Filename = filename,
            Width = width,
            Height = height
        };
    }
}

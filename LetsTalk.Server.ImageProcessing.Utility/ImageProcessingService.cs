using LetsTalk.Server.FileStorage.AgnosticServices.Abstractions;
using LetsTalk.Server.ImageProcessing.ImageResizeEngine.Abstractions;
using LetsTalk.Server.ImageProcessing.Utility.Abstractions;
using LetsTalk.Server.ImageProcessing.Utility.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.ImageProcessing.Utility;

public class ImageProcessingService(
    IImageService imageService,
    IFileService fileService,
    IImageResizeService imageResizeService) : IImageProcessingService
{
    private readonly IImageService _imageService = imageService;
    private readonly IFileService _fileService = fileService;
    private readonly IImageResizeService _imageResizeService = imageResizeService;

    public async Task<ProcessImageResponse> ProcessImageAsync(string imageId, int maxWidth, int maxHeight, CancellationToken cancellationToken = default)
    {
        var fetchImageResponse = await _imageService.FetchImageAsync(imageId, cancellationToken);

        var (data, width, height) = _imageResizeService.Resize(
            fetchImageResponse!.Content!,
            maxWidth,
            maxHeight);

        var filename = await _fileService.SaveDataAsync(data!, FileTypes.Image, width, height, cancellationToken: cancellationToken);
        await _fileService.SaveImageInfoAsync(filename, width, height, cancellationToken);

        return new ProcessImageResponse
        {
            Filename = filename,
            Width = width,
            Height = height
        };
    }
}

using LetsTalk.Server.FileStorage.AgnosticServices.Abstractions;
using LetsTalk.Server.ImageProcessing.ImageResizeEngine.Abstractions;
using LetsTalk.Server.ImageProcessing.Utility.Abstractions;
using LetsTalk.Server.ImageProcessing.Utility.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.ImageProcessing.Utility;

public class ImageProcessingService(
    IAgnosticFileService fileService,
    IImageResizeService imageResizeService) : IImageProcessingService
{
    private readonly IAgnosticFileService _fileService = fileService;
    private readonly IImageResizeService _imageResizeService = imageResizeService;

    public async Task<ProcessImageResponse> ProcessImageAsync(string imageId, int maxWidth, int maxHeight, CancellationToken cancellationToken = default)
    {
        var content = await _fileService.ReadFileAsync(imageId, FileTypes.Image, cancellationToken);

        var (data, width, height) = _imageResizeService.Resize(
            content,
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

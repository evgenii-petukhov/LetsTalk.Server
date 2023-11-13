using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.FileStorage.Utility.Abstractions.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorage.Utility;

public class ImageService : IImageService
{
    private readonly IFileService _fileService;
    private readonly IImageAgnosticService _imageAgnosticService;

    public ImageService(
        IFileService fileService,
        IImageAgnosticService imageAgnosticService)
    {
        _fileService = fileService;
        _imageAgnosticService = imageAgnosticService;
    }

    public async Task<FetchImageResponse> FetchImageAsync(int imageId, CancellationToken cancellationToken = default)
    {
        var image = await _imageAgnosticService.GetByIdWithFileAsync(imageId, cancellationToken);

        return new FetchImageResponse
        {
            Content = await _fileService.ReadFileAsync(image!.File!.FileName!, FileTypes.Image, cancellationToken),
            Width = image.Width ?? 0,
            Height = image.Height ?? 0,
            ImageRole = (ImageRoles)image.ImageRoleId
        };
    }
}

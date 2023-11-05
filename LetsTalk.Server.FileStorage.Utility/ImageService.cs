using LetsTalk.Server.FileStorage.Utility.Abstractions;
using LetsTalk.Server.FileStorage.Utility.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.FileStorage.Utility;

public class ImageService : IImageService
{
    private readonly IFileService _fileService;
    private readonly IImageRepository _imageRepository;

    public ImageService(
        IFileService fileService,
        IImageRepository imageRepository)
    {
        _fileService = fileService;
        _imageRepository = imageRepository;
    }

    public async Task<FetchImageResponse> FetchImageAsync(int imageId, bool useDimensions = false, CancellationToken cancellationToken = default)
    {
        dynamic? image = useDimensions
            ? await _imageRepository.GetByIdWithFileAsync(imageId, x => new
            {
                x.File!.FileName,
                x.Width,
                x.Height,
                x.ImageRoleId
            }, cancellationToken)
            : await _imageRepository.GetByIdWithFileAsync(imageId, x => new
            {
                x.File!.FileName,
                Width = 0,
                Height = 0,
                x.ImageRoleId
            }, cancellationToken);

        return new FetchImageResponse
        {
            Content = await _fileService.ReadFileAsync(image!.FileName!, FileTypes.Image, cancellationToken),
            Width = image.Width ?? 0,
            Height = image.Height ?? 0,
            ImageRole = (ImageRoles)image.ImageRoleId
        };
    }
}

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

    public async Task<byte[]> FetchImageAsync(int imageId, CancellationToken cancellationToken = default)
    {
        var filename = await _imageDataLayerService.GetByIdOrDefaultAsync(imageId, x => x.File!.FileName, cancellationToken);

        return await _fileService.ReadFileAsync(filename!, FileTypes.Image, cancellationToken);
    }
}

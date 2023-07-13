using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.FileStorageService.Abstractions;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.FileStorageService.Services;

public class ImageService : IImageService
{
    private readonly IFileService _fileService;
    private readonly IFileRepository _fileRepository;
    private readonly IAccountDataLayerService _accountDataLayerService;
    private readonly IImageDataLayerService _imageDataLayerService;
    private readonly IMemoryCache _memoryCache;
    private readonly FileStorageSettings _fileStorageSettings;

    public ImageService(
        IFileService fileService,
        IFileRepository fileRepository,
        IAccountDataLayerService accountDataLayerService,
        IImageDataLayerService imageDataLayerService,
        IMemoryCache memoryCache,
        IOptions<FileStorageSettings> options)
    {
        _fileService = fileService;
        _fileRepository = fileRepository;
        _accountDataLayerService = accountDataLayerService;
        _imageDataLayerService = imageDataLayerService;
        _memoryCache = memoryCache;
        _fileStorageSettings = options.Value;
    }

    public async Task<int> SaveImageAsync(byte[] content, ImageRoles imageRole, ImageFormats imageFormat, int accountId, CancellationToken cancellationToken = default)
    {
        if (imageRole == ImageRoles.Avatar)
        {
            await RemoveAvatarAsync(accountId, cancellationToken);
        }

        var data = content.ToArray();
        var filename = await _fileService.SaveDataAsync(data, FileTypes.Image, imageRole, cancellationToken);
        var imageId = await _imageDataLayerService.CreateWithFileAsync(filename, imageFormat, imageRole, cancellationToken);

        _memoryCache.Set(imageId, filename, _fileStorageSettings.FilenameByImageIdCacheLifetime);

        return imageId;
    }

    public async Task<byte[]> FetchImageAsync(int imageId, CancellationToken cancellationToken = default)
    {
        var filename = await _memoryCache.GetOrCreateAsync(imageId, async cacheEntry =>
        {
            cacheEntry.AbsoluteExpirationRelativeToNow = _fileStorageSettings.FilenameByImageIdCacheLifetime;
            return await _imageDataLayerService.GetByIdOrDefaultAsync(imageId, x => x.File!.FileName, cancellationToken);
        });

        return await _fileService.ReadFileAsync(filename!, FileTypes.Image, cancellationToken);
    }

    private async Task RemoveAvatarAsync(int accountId, CancellationToken cancellationToken = default)
    {
        var response = await _accountDataLayerService.GetByIdOrDefaultAsync(accountId, x => new
        {
            x.ImageId,
            x.Image!.File
        }, true, cancellationToken);
        if (!response?.ImageId.HasValue ?? true)
        {
            return;
        }

        _memoryCache.Remove(response!.ImageId!);
        if (response!.File != null)
        {
            _fileService.DeleteFile(response!.File.FileName!, FileTypes.Image);
            await _fileRepository.DeleteAsync(response.File.Id, cancellationToken);
        }
    }
}

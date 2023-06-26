using LetsTalk.Server.Domain;
using LetsTalk.Server.FileStorageService.Abstractions;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.Extensions.Caching.Memory;

namespace LetsTalk.Server.FileStorageService.Services;

public class ImageService : IImageService
{
    private readonly IFileService _fileService;
    private readonly IImageRepository _imageRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IImageInfoService _imageInfoService;
    private readonly IAccountRepository _accountRepository;
    private readonly IMemoryCache _memoryCache;

    public ImageService(
        IFileService fileService,
        IImageRepository imageRepository,
        IFileRepository fileRepository,
        IImageInfoService imageInfoService,
        IAccountRepository accountRepository,
        IMemoryCache memoryCache)
    {
        _fileService = fileService;
        _imageRepository = imageRepository;
        _fileRepository = fileRepository;
        _imageInfoService = imageInfoService;
        _accountRepository = accountRepository;
        _memoryCache = memoryCache;
    }

    public async Task SaveImageAsync(byte[] content, ImageTypes imageType, int accountId, CancellationToken cancellationToken = default)
    {
        if (imageType == ImageTypes.Avatar)
        {
            await RemoveAvatarAsync(accountId, cancellationToken);
        }

        var data = content.ToArray();
        var filename = await _fileService.SaveDataAsync(data, FileTypes.Image, cancellationToken);

        var file = await _fileRepository.CreateAsync(new Domain.File
        {
            FileName = filename,
            FileTypeId = (int)FileTypes.Image,
        }, cancellationToken);

        var image = await _imageRepository.CreateAsync(new Image
        {
            ImageContentTypeId = (int)_imageInfoService.GetImageContentType(data),
            ImageTypeId = (int)imageType,
            FileId = file.Id
        }, cancellationToken);

        _memoryCache.Set(image.Id, file.FileName);

        if (imageType == ImageTypes.Avatar)
        {
            await _accountRepository.SetImageIdAsync(accountId, image.Id, cancellationToken);
        }
    }

    public async Task<byte[]> FetchImageAsync(int imageId, CancellationToken cancellationToken = default)
    {
        var filename = await _memoryCache.GetOrCreateAsync(imageId, async cacheEntry =>
        {
            cacheEntry.Priority = CacheItemPriority.NeverRemove;
            var image = await _imageRepository.GetByIdWithFileAsync(imageId, cancellationToken);
            return image!.File!.FileName;
        });

        return await _fileService.ReadFileAsync(filename!, FileTypes.Image, cancellationToken);
    }

    private async Task RemoveAvatarAsync(int accountId, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdIncludingFilesAsync(accountId, cancellationToken);
        var imageId = account?.ImageId;
        if (!imageId.HasValue)
        {
            return;
        }

        _memoryCache.Remove(imageId);
        var file = account!.Image?.File;
        if (file != null)
        {
            _fileService.DeleteFile(file!.FileName!, FileTypes.Image);
            await _fileRepository.DeleteAsync(file.Id, cancellationToken);
        }
    }
}

using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.FileStorage.Models;
using LetsTalk.Server.FileStorageService.Abstractions;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.FileStorageService.Services;

public class FileManagementService : IFileManagementService
{
    private readonly IFileNameGenerator _fileNameGenerator;
    private readonly IImageInfoService _imageInfoService;
    private readonly FileStorageSettings _fileStorageSettings;

    public FileManagementService(
        IFileNameGenerator fileNameGenerator,
        IImageInfoService imageInfoService,
        IOptions<FileStorageSettings> options)
    {
        _fileNameGenerator = fileNameGenerator;
        _imageInfoService = imageInfoService;
        _fileStorageSettings = options.Value;
    }

    public Task<byte[]> GetFileContentAsync(string filename, FileStorageItemType fileType, CancellationToken cancellationToken = default)
    {
        var imagePath = _fileNameGenerator.GetFilePath(filename, fileType);
        return File.ReadAllBytesAsync(imagePath, cancellationToken);
    }

    public async Task<FilePathInfo> SaveFileAsync(byte[] data, FileStorageItemType fileType, CancellationToken cancellationToken = default)
    {
        if (fileType == FileStorageItemType.Image)
        {
            var size = _imageInfoService.GetImageSize(data);
            if (size.Width > _fileStorageSettings.AvatarMaxWidth || size.Height > _fileStorageSettings.AvatarMaxWidth)
            {
                throw new ImageSizeException("Image size exceeds max dimensions");
            }
        }

        var filePathInfo = _fileNameGenerator.Generate(fileType);
        await File.WriteAllBytesAsync(filePathInfo.FullPath!, data, cancellationToken);
        return filePathInfo;
    }

    public void DeleteFile(string filename, FileStorageItemType fileType)
    {
        var path = _fileNameGenerator.GetFilePath(filename, fileType);
        File.Delete(path);
    }
}

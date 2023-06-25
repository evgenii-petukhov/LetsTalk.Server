using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.FileStorageService.Abstractions;
using Microsoft.Extensions.Options;
using LetsTalk.Server.Persistence.Enums;

namespace LetsTalk.Server.FileStorageService.Services;

public class FileService : IFileService
{
    private readonly IFileNameGenerator _fileNameGenerator;
    private readonly IImageInfoService _imageInfoService;
    private readonly FileStorageSettings _fileStorageSettings;

    public FileService(
        IFileNameGenerator fileNameGenerator,
        IImageInfoService imageInfoService,
        IOptions<FileStorageSettings> options)
    {
        _fileNameGenerator = fileNameGenerator;
        _imageInfoService = imageInfoService;
        _fileStorageSettings = options.Value;
    }

    public Task<byte[]> ReadFileAsync(string filename, FileTypes fileType, CancellationToken cancellationToken = default)
    {
        var imagePath = _fileNameGenerator.GetFilePath(filename, fileType);
        return File.ReadAllBytesAsync(imagePath, cancellationToken);
    }

    public async Task<string?> SaveDataAsync(byte[] data, FileTypes fileType, CancellationToken cancellationToken = default)
    {
        if (fileType == FileTypes.Image)
        {
            var (width, height) = _imageInfoService.GetImageSize(data);
            if (width > _fileStorageSettings.AvatarMaxWidth || height > _fileStorageSettings.AvatarMaxWidth)
            {
                throw new ImageSizeException("Image size exceeds max dimensions");
            }
        }

        var (filename, filepath) = _fileNameGenerator.Generate(fileType);
        await File.WriteAllBytesAsync(filepath, data, cancellationToken);
        return filename;
    }

    public void DeleteFile(string filename, FileTypes fileType)
    {
        var path = _fileNameGenerator.GetFilePath(filename, fileType);
        File.Delete(path);
    }
}

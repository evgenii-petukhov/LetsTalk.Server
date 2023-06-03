using LetsTalk.Server.FileStorage.Abstractions;
using LetsTalk.Server.FileStorage.Models;
using LetsTalk.Server.Persistence.Models;

namespace LetsTalk.Server.FileStorage.Services;

public class FileStorageManager : IFileStorageManager
{
    private readonly IImageFileNameGenerator _imageFileNameGenerator;

    public FileStorageManager(
        IImageFileNameGenerator imageFileNameGenerator)
    {
        _imageFileNameGenerator = imageFileNameGenerator;
    }

    public Task<byte[]> GetImageContentAsync(string filename, CancellationToken cancellationToken)
    {
        var imagePath = _imageFileNameGenerator.GetImagePath(filename);
        return File.ReadAllBytesAsync(imagePath, cancellationToken);
    }

    public async Task<FilePathInfo> SaveImageAsync(byte[] data, ImageContentTypes contentType, CancellationToken cancellationToken)
    {
        var filePathInfo = _imageFileNameGenerator.Generate(contentType);
        await File.WriteAllBytesAsync(filePathInfo.FullPath!, data, cancellationToken);
        return filePathInfo;
    }

    public void DeleteImage(string filename)
    {
        var path = _imageFileNameGenerator.GetImagePath(filename);
        File.Delete(path);
    }
}

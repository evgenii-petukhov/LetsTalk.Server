using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LetsTalk.Server.Persistence.Services;

public class ImageDataLayerService : IImageDataLayerService
{
    private readonly IImageRepository _imageRepository;
    private readonly IFileRepository _fileRepository;

    public ImageDataLayerService(
        IImageRepository imageRepository,
        IFileRepository fileRepository)
    {
        _imageRepository = imageRepository;
        _fileRepository = fileRepository;
    }

    public Task<T?> GetByIdOrDefaultAsync<T>(int id, Expression<Func<Image, T>> selector, CancellationToken cancellationToken = default)
    {
        return _imageRepository.GetById(id)
            .Select(selector)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async Task<int> CreateWithFileAsync(string filename, ImageFormats imageFormat, ImageRoles imageRole, CancellationToken cancellationToken = default)
    {
        var file = new Domain.File
        {
            FileName = filename,
            FileTypeId = (int)FileTypes.Image,
        };
        await _fileRepository.CreateAsync(file, cancellationToken);

        var image = new Image
        {
            ImageFormatId = (int)imageFormat,
            ImageRoleId = (int)imageRole,
            FileId = file.Id
        };
        await _imageRepository.CreateAsync(image, cancellationToken);

        return image.Id;
    }
}

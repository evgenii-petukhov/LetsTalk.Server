using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LetsTalk.Server.Persistence.Services;

public class ImageDataLayerService : IImageDataLayerService
{
    private readonly IImageRepository _imageRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly LetsTalkDbContext _context;

    public ImageDataLayerService(
        IImageRepository imageRepository,
        IFileRepository fileRepository,
        IMessageRepository messageRepository,
        LetsTalkDbContext context)
    {
        _imageRepository = imageRepository;
        _fileRepository = fileRepository;
        _messageRepository = messageRepository;
        _context = context;
    }

    public Task<T?> GetByIdOrDefaultAsync<T>(
        int id,
        Expression<Func<Image, T>> selector,
        CancellationToken cancellationToken = default)
    {
        return _imageRepository.GetById(id)
            .Select(selector)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Image> CreateImagePreviewAsync(
        string filename,
        ImageFormats imageFormat,
        int width,
        int height,
        int messageId,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        var image = await CreateImageInternalAsync(filename, imageFormat, ImageRoles.Message, width, height, cancellationToken);
        await _messageRepository.SetImagePreviewAsync(messageId, image.Id, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return image;
    }

    public async Task<Image> ReplaceImageAsync(
        string filename,
        ImageFormats imageFormat,
        ImageRoles imageRole,
        int width,
        int height,
        int prevImageId,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        var image = await CreateImageInternalAsync(filename, imageFormat, imageRole, width, height, cancellationToken);
        await _fileRepository.DeleteAsync(prevImageId, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return image;
    }

    public async Task<Image> CreateImageAsync(string filename, ImageFormats imageFormat, ImageRoles imageRole, int width, int height, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        var image = await CreateImageInternalAsync(filename, imageFormat, ImageRoles.Avatar, width, height, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return image;
    }

    private async Task<Image> CreateImageInternalAsync(
        string filename,
        ImageFormats imageFormat,
        ImageRoles imageRole,
        int width,
        int height,
        CancellationToken cancellationToken = default)
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
            FileId = file.Id,
            Width = width,
            Height = height
        };
        await _imageRepository.CreateAsync(image, cancellationToken);

        return image;
    }
}

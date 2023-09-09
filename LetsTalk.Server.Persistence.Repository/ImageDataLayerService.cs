using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.Repository;

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

    public async Task<Image> CreateImageAsync(string filename, ImageFormats imageFormat, ImageRoles imageRole, 
        int width, int height, CancellationToken cancellationToken = default)
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
        var file = new Domain.File(filename, (int)FileTypes.Image);
        var image = new Image((int)imageFormat, (int)imageRole, width, height);
        file.AddImage(image);
        await _fileRepository.CreateAsync(file, cancellationToken);

        return image;
    }
}

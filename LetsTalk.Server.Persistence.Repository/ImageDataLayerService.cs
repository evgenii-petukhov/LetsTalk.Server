using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.Repository;

public class ImageDataLayerService : IImageDataLayerService
{
    private readonly IFileRepository _fileRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ImageDataLayerService(
        IFileRepository fileRepository,
        IMessageRepository messageRepository,
        IUnitOfWork unitOfWork)
    {
        _fileRepository = fileRepository;
        _messageRepository = messageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Image> CreateImagePreviewAsync(
        string filename,
        ImageFormats imageFormat,
        int width,
        int height,
        int messageId,
        CancellationToken cancellationToken = default)
    {
        var image = await CreateImageInternalAsync(filename, imageFormat, ImageRoles.Message, width, height, cancellationToken);
        var message = await _messageRepository.GetByIdAsTrackingAsync(messageId, cancellationToken);
        message.SetImagePreview(image);
        await _unitOfWork.SaveAsync(cancellationToken);

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
        var image = await CreateImageInternalAsync(filename, imageFormat, imageRole, width, height, cancellationToken);
        await _fileRepository.DeleteAsync(prevImageId, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return image;
    }

    public async Task<Image> CreateImageAsync(string filename, ImageFormats imageFormat, ImageRoles imageRole, 
        int width, int height, CancellationToken cancellationToken = default)
    {
        var image = await CreateImageInternalAsync(filename, imageFormat, ImageRoles.Avatar, width, height, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);
        return image;
    }

    private async Task<Image> CreateImageInternalAsync(
        string filename,
        ImageFormats imageFormat,
        ImageRoles imageRole,
        int width,
        int height,
        CancellationToken cancellationToken)
    {
        var file = new Domain.File(filename, (int)FileTypes.Image);
        var image = new Image((int)imageFormat, (int)imageRole, width, height);
        file.AttachImage(image);
        await _fileRepository.CreateAsync(file, cancellationToken);

        return image;
    }
}

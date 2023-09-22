using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.Repository;

public class ImageDataLayerService : IImageDataLayerService
{
    private readonly IFileRepository _fileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEntityFactory _entityFactory;
    private readonly IMessageRepository _messageRepository;

    public ImageDataLayerService(
        IFileRepository fileRepository,
        IUnitOfWork unitOfWork,
        IEntityFactory entityFactory,
        IMessageRepository messageRepository)
    {
        _fileRepository = fileRepository;
        _unitOfWork = unitOfWork;
        _entityFactory = entityFactory;
        _messageRepository = messageRepository;
    }

    public async Task CreateImagePreviewAsync(
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
        var file = _entityFactory.CreateFile(filename, FileTypes.Image);
        var image = _entityFactory.CreateImage(imageFormat, imageRole, width, height);
        file.SetImage(image);
        await _fileRepository.CreateAsync(file, cancellationToken);

        return image;
    }
}

using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.Repository;

public class ImageDataLayerService : IImageDataLayerService
{
    private readonly IFileRepository _fileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEntityFactory _entityFactory;

    public ImageDataLayerService(
        IFileRepository fileRepository,
        IUnitOfWork unitOfWork,
        IEntityFactory entityFactory)
    {
        _fileRepository = fileRepository;
        _unitOfWork = unitOfWork;
        _entityFactory = entityFactory;
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
        var message = _entityFactory.CreateMessage(messageId);
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
        var file = _entityFactory.CreateFile(prevImageId);
        _fileRepository.Delete(file);
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
        var file = _entityFactory.CreateFile(filename, FileTypes.Image);
        var image = _entityFactory.CreateImage(imageFormat, imageRole, width, height);
        file.SetImage(image);
        await _fileRepository.CreateAsync(file, cancellationToken);

        return image;
    }
}

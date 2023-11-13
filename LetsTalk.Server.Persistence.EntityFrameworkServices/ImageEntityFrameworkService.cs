using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.EntityFrameworkServices;

public class ImageEntityFrameworkService : IImageAgnosticService
{
    private readonly IImageRepository _imageRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IEntityFactory _entityFactory;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ImageEntityFrameworkService(
        IImageRepository imageRepository,
        IFileRepository fileRepository,
        IMessageRepository messageRepository,
        IEntityFactory entityFactory,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _imageRepository = imageRepository;
        _fileRepository = fileRepository;
        _messageRepository = messageRepository;
        _entityFactory = entityFactory;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public Task<bool> IsImageIdValidAsync(int id, CancellationToken cancellationToken = default)
    {
        return _imageRepository.IsImageIdValidAsync(id, cancellationToken);
    }

    public async Task<ImageServiceModel?> GetByIdWithFileAsync(int id, CancellationToken cancellationToken = default)
    {
        var image = await _imageRepository.GetByIdWithFileAsync(id, cancellationToken);

        return _mapper.Map<ImageServiceModel>(image);
    }

    public async Task<ImageServiceModel> CreateImageAsync(
        string filename,
        ImageFormats imageFormat,
        ImageRoles imageRole,
        int width,
        int height,
        CancellationToken cancellationToken = default)
    {
        var image = await CreateImageInternalAsync(filename, imageFormat, imageRole, width, height, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return _mapper.Map<ImageServiceModel>(image);
    }

    public async Task<MessageServiceModel> SaveImagePreviewAsync(
        int messageId,
        string filename,
        ImageFormats imageFormat,
        ImageRoles imageRole,
        int width,
        int height,
        CancellationToken cancellationToken = default)
    {
        var image = await CreateImageInternalAsync(filename, imageFormat, imageRole, width, height, cancellationToken);
        var message = await _messageRepository.GetByIdAsTrackingAsync(messageId, cancellationToken);
        message.SetImagePreview(image);

        await _unitOfWork.SaveAsync(cancellationToken);

        return _mapper.Map<MessageServiceModel>(message);
    }

    private async Task<Domain.Image> CreateImageInternalAsync(
        string filename,
        ImageFormats imageFormat,
        ImageRoles imageRole,
        int width,
        int height,
        CancellationToken cancellationToken = default)
    {
        var file = _entityFactory.CreateFile(filename, FileTypes.Image);
        var image = _entityFactory.CreateImage(imageFormat, imageRole, width, height);
        file.SetImage(image);
        await _fileRepository.CreateAsync(file, cancellationToken);

        return image;
    }
}

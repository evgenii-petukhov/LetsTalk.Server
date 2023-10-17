using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.Repository;

public class ImageDomainService : IImageDomainService
{
    private readonly IFileRepository _fileRepository;
    private readonly IEntityFactory _entityFactory;
    private readonly IMessageDomainService _messageDomainService;

    public ImageDomainService(
        IFileRepository fileRepository,
        IEntityFactory entityFactory,
        IMessageDomainService messageDomainService)
    {
        _fileRepository = fileRepository;
        _entityFactory = entityFactory;
        _messageDomainService = messageDomainService;
    }

    public async Task CreateImagePreviewAsync(
        string filename,
        ImageFormats imageFormat,
        int width,
        int height,
        int messageId)
    {
        var image = await CreateImageInternalAsync(filename, imageFormat, ImageRoles.Message, width, height);
        await _messageDomainService.SetImagePreviewAsync(image, messageId);
    }

    public Task<Image> CreateImageAsync(string filename, ImageFormats imageFormat, ImageRoles imageRole,
        int width, int height, CancellationToken cancellationToken)
    {
        return CreateImageInternalAsync(filename, imageFormat, ImageRoles.Avatar, width, height, cancellationToken);
    }

    private async Task<Image> CreateImageInternalAsync(
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

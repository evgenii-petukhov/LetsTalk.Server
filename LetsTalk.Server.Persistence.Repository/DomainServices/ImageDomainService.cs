using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.Repository.DomainServices;

public class ImageDomainService : IImageDomainService
{
    private readonly IFileRepository _fileRepository;
    private readonly IEntityFactory _entityFactory;

    public ImageDomainService(
        IFileRepository fileRepository,
        IEntityFactory entityFactory)
    {
        _fileRepository = fileRepository;
        _entityFactory = entityFactory;
    }

    public async Task<Image> CreateImageAsync(
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

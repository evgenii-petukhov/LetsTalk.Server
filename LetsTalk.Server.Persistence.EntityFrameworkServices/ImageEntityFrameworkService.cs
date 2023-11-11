using LetsTalk.Server.Persistence.DatabaseAgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.EntityFrameworkServices;

public class ImageEntityFrameworkService : IImageDatabaseAgnosticService
{
    private readonly IImageRepository _imageRepository;

    public ImageEntityFrameworkService(IImageRepository imageRepository)
    {
        _imageRepository = imageRepository;
    }

    public Task<bool> IsImageIdValidAsync(int id, CancellationToken cancellationToken = default)
    {
        return _imageRepository.IsImageIdValidAsync(id, cancellationToken);
    }
}

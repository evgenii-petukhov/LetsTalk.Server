using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.EntityFrameworkServices;

public class ImageEntityFrameworkService : IImageAgnosticService
{
    private readonly IImageRepository _imageRepository;
    private readonly IMapper _mapper;

    public ImageEntityFrameworkService(
        IImageRepository imageRepository,
        IMapper mapper)
    {
        _imageRepository = imageRepository;
        _mapper = mapper;
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
}

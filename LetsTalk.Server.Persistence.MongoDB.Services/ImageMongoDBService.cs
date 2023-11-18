using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.MongoDB.Services;

public class ImageMongoDBService : IImageAgnosticService
{
    private readonly IImageRepository _imageRepository;
    private readonly IMapper _mapper;

    public ImageMongoDBService(
        IImageRepository imageRepository,
        IMapper mapper)
    {
        _imageRepository = imageRepository;
        _mapper = mapper;
    }

    public Task<bool> IsImageIdValidAsync(string id, CancellationToken cancellationToken = default)
    {
        return _imageRepository.IsImageIdValidAsync(id, cancellationToken);
    }

    public Task<ImageServiceModel?> GetByIdWithFileAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ImageServiceModel> CreateImageAsync(
        string filename,
        ImageFormats imageFormat,
        ImageRoles imageRole,
        int width,
        int height,
        CancellationToken cancellationToken = default)
    {
        var image = await _imageRepository.CreateImageAsync(filename, imageFormat, imageRole, width, height, cancellationToken);

        return _mapper.Map<ImageServiceModel>(image);
    }

    public Task<MessageServiceModel> SaveImagePreviewAsync(
        string messageId,
        string filename,
        ImageFormats imageFormat,
        ImageRoles imageRole,
        int width,
        int height,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

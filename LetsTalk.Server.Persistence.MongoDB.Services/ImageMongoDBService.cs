using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.MongoDB.Services;

public class ImageMongoDBService : IImageAgnosticService
{
    private readonly IUploadRepository _uploadRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public ImageMongoDBService(
        IUploadRepository uploadRepository,
        IMessageRepository messageRepository,
        IMapper mapper)
    {
        _uploadRepository = uploadRepository;
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    public Task<bool> IsImageIdValidAsync(string id, CancellationToken cancellationToken = default)
    {
        return _uploadRepository.IsImageIdValidAsync(id, cancellationToken);
    }

    public async Task<ImageServiceModel?> GetByIdWithFileAsync(string id, CancellationToken cancellationToken = default)
    {
        var image = await _uploadRepository.GetByIdWithFileAsync(id, cancellationToken);

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
        var image = await _uploadRepository.CreateImageAsync(filename, imageFormat, imageRole, width, height, cancellationToken);

        return _mapper.Map<ImageServiceModel>(image);
    }

    public async Task<MessageServiceModel> SaveImagePreviewAsync(
        string messageId,
        string filename,
        ImageFormats imageFormat,
        ImageRoles imageRole,
        int width,
        int height,
        CancellationToken cancellationToken = default)
    {
        var image = await _uploadRepository.CreateImageAsync(filename, imageFormat, imageRole, width, height, cancellationToken);

        var message = await _messageRepository.SetImagePreviewAsync(messageId, image.Id!, cancellationToken);

        message.ImagePreview = image;

        return _mapper.Map<MessageServiceModel>(message);
    }
}

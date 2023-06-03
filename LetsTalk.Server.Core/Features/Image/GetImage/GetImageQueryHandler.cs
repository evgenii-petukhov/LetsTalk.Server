using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.FileStorage.Abstractions;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Image.GetImage;

public class GetImageQueryHandler : IRequestHandler<GetImageQuery, ImageDto>
{
    private readonly IImageRepository _imageRepository;
    private readonly IBase64ParsingService _base64ParsingService;
    private readonly IFileStorageManager _fileStorageManager;

    public GetImageQueryHandler(
        IImageRepository imageRepository,
        IBase64ParsingService base64ParsingService,
        IFileStorageManager fileStorageManager)
    {
        _imageRepository = imageRepository;
        _base64ParsingService = base64ParsingService;
        _fileStorageManager = fileStorageManager;
    }

    public async Task<ImageDto> Handle(GetImageQuery request, CancellationToken cancellationToken)
    {
        var image = await _imageRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw new BadRequestException("Invalid request");
        var content = await _fileStorageManager.GetImageContentAsync(image.FileName!, cancellationToken);

        return new ImageDto
        {
            Content = _base64ParsingService.CreateBase64String(content, (ImageContentTypes)image.ImageContentTypeId)
        };
    }
}

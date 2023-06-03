using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Image.GetImage;

public class GetImageQueryHandler : IRequestHandler<GetImageQuery, ImageDto>
{
    private readonly IImageRepository _imageRepository;
    private readonly IImageFileNameGenerator _imageFileNameGenerator;
    private readonly IBase64ParsingService _base64ParsingService;

    public GetImageQueryHandler(
        IImageRepository imageRepository,
        IImageFileNameGenerator imageFileNameGenerator,
        IBase64ParsingService base64ParsingService)
    {
        _imageRepository = imageRepository;
        _imageFileNameGenerator = imageFileNameGenerator;
        _base64ParsingService = base64ParsingService;
    }

    public async Task<ImageDto> Handle(GetImageQuery request, CancellationToken cancellationToken)
    {
        var image = await _imageRepository.GetByIdAsync(request.Id) ?? throw new BadRequestException("Invalid request");

        var imagePath = _imageFileNameGenerator.GetImagePath(image.FileName!);
        var bytes = await File.ReadAllBytesAsync(imagePath, cancellationToken);
        return new ImageDto
        {
            Content = _base64ParsingService.CreateBase64String(bytes, (ImageContentTypes)image.ImageContentTypeId)
        };
    }
}

using AutoMapper;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.FileStorage.Abstractions;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.Models;
using MediatR;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Core.Features.Profile.Commands.UpdateProfileCommand;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, AccountDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IImageRepository _imageRepository;
    private readonly IBase64ParsingService _base64ParsingService;
    private readonly IAccountDataLayerService _accountDataLayerService;
    private readonly IFileStorageManager _fileStorageManager;
    private readonly IMapper _mapper;
    private readonly IImageService _imageService;
    private readonly FileStorageSettings _fileStorageSettings;

    public UpdateProfileCommandHandler(
        IAccountRepository accountRepository,
        IImageRepository imageRepository,
        IBase64ParsingService base64ParsingService,
        IAccountDataLayerService accountDataLayerService,
        IFileStorageManager fileStorageManager,
        IMapper mapper,
        IImageService imageService,
        IOptions<FileStorageSettings> fileStorageSettings)
    {
        _accountRepository = accountRepository;
        _imageRepository = imageRepository;
        _base64ParsingService = base64ParsingService;
        _accountDataLayerService = accountDataLayerService;
        _fileStorageManager = fileStorageManager;
        _mapper = mapper;
        _imageService = imageService;
        _fileStorageSettings = fileStorageSettings.Value;
    }

    public async Task<AccountDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateProfileCommandValidator(_accountRepository);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid request", validationResult);
        }

        var base64ParsingResult = _base64ParsingService.ParseBase64String(request.PhotoUrl);
        if (base64ParsingResult == null)
        {
            await _accountRepository.UpdateAsync(request.AccountId!.Value, request.FirstName, request.LastName, request.Email, cancellationToken);
        }
        else
        {
            var data = Convert.FromBase64String(base64ParsingResult.Base64string!);
            var imageInfo = _imageService.GetImageInfo(data);
            if (imageInfo.Width > _fileStorageSettings.AvatarMaxWidth || imageInfo.Height > _fileStorageSettings.AvatarMaxWidth)
            {
                throw new BadRequestException("Image size exceeds max dimensions");
            }

            var filePathInfo = await _fileStorageManager.SaveImageAsync(data, base64ParsingResult.ImageContentType, cancellationToken);
            var image = await _imageRepository.CreateAsync(new Domain.Image
            {
                FileName = filePathInfo.FileName,
                ImageContentTypeId = (int)base64ParsingResult.ImageContentType,
                ImageTypeId = (int)ImageTypes.Avatar
            }, cancellationToken);
            await _accountDataLayerService.UpdateAsync(request.AccountId!.Value, request.FirstName, request.LastName, request.Email, image.Id, cancellationToken);
        }

        var account = await _accountRepository.GetByIdAsync(request.AccountId!.Value, cancellationToken);

        return _mapper.Map<AccountDto>(account);
    }
}

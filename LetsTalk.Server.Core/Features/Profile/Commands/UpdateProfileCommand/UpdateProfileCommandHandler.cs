using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Profile.Commands.UpdateProfileCommand;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, AccountDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IImageRepository _imageRepository;
    private readonly IBase64ParsingService _base64ParsingService;
    private readonly IImageFileNameGenerator _imageFileNameGenerator;
    private readonly IAccountDataLayerService _accountDataLayerService;
    private readonly IMapper _mapper;

    public UpdateProfileCommandHandler(
        IAccountRepository accountRepository,
        IImageRepository imageRepository,
        IBase64ParsingService base64ParsingService,
        IImageFileNameGenerator imageFileNameGenerator,
        IAccountDataLayerService accountDataLayerService,
        IMapper mapper)
    {
        _accountRepository = accountRepository;
        _imageRepository = imageRepository;
        _base64ParsingService = base64ParsingService;
        _imageFileNameGenerator = imageFileNameGenerator;
        _accountDataLayerService = accountDataLayerService;
        _mapper = mapper;
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
            await _accountRepository.UpdateAsync(request.AccountId!.Value, request.FirstName, request.LastName, request.Email);
        }
        else
        {
            var data = Convert.FromBase64String(base64ParsingResult.Base64string!);
            var filePathInfo = _imageFileNameGenerator.Generate(base64ParsingResult.ImageContentType);
            await File.WriteAllBytesAsync(filePathInfo.FullPath!, data, cancellationToken);
            var image = await _imageRepository.CreateAsync(new Domain.Image
            {
                FileName = filePathInfo.FileName,
                ImageContentTypeId = (int)base64ParsingResult.ImageContentType,
                ImageTypeId = (int)ImageTypes.Avatar
            });
            await _accountDataLayerService.UpdateAsync(request.AccountId!.Value, request.FirstName, request.LastName, request.Email, image.Id);
        }

        var account = await _accountRepository.GetByIdAsync(request.AccountId!.Value);

        return _mapper.Map<AccountDto>(account);
    }
}

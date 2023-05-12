using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Core.Helpers;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Persistence.Abstractions;
using MediatR;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Core.Features.Profile.Commands.UpdateProfileCommand;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IImageTypeService _imageTypeService;
    private readonly IBase64ParsingService _base64ParsingService;
    private readonly FileStorageSettings _fileStorageSettings;

    public UpdateProfileCommandHandler(
        IAccountRepository accountRepository,
        IImageTypeService imageTypeService,
        IBase64ParsingService base64ParsingService,
        IOptions<FileStorageSettings> fileStorageSettings)
    {
        _accountRepository = accountRepository;
        _imageTypeService = imageTypeService;
        _base64ParsingService = base64ParsingService;
        _fileStorageSettings = fileStorageSettings.Value;
    }

    public async Task Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var validator = new UpdateProfileCommandValidator(_accountRepository);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Invalid request", validationResult);
        }

        var base64ParsingResult = _base64ParsingService.ParseBase64Image(request.PhotoUrl);
        if (base64ParsingResult == null)
        {
            await _accountRepository.UpdateAsync(request.AccountId!.Value, request.FirstName, request.LastName, request.Email);
        }
        else
        {
            var data = Convert.FromBase64String(base64ParsingResult.Base64string!);
            var extension = _imageTypeService.GetExtensionByImageType(base64ParsingResult.Imagetype);
            var filename = Path.Combine(_fileStorageSettings.BasePath!, _fileStorageSettings.ImageFolder!, Guid.NewGuid().ToString() + extension);
            filename = Environment.ExpandEnvironmentVariables(filename);
            await File.WriteAllBytesAsync(filename, data, cancellationToken);
            await _accountRepository.UpdateAsync(request.AccountId!.Value, request.FirstName, request.LastName, request.Email, request.PhotoUrl);
        }
    }
}

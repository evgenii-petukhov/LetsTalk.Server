using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Persistence.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Profile.Commands.UpdateProfileCommand;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IBase64ParsingService _base64ParsingService;
    private readonly IImageFileNameGenerator _imageFileNameGenerator;

    public UpdateProfileCommandHandler(
        IAccountRepository accountRepository,
        IBase64ParsingService base64ParsingService,
        IImageFileNameGenerator imageFileNameGenerator)
    {
        _accountRepository = accountRepository;
        _base64ParsingService = base64ParsingService;
        _imageFileNameGenerator = imageFileNameGenerator;
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
            var filename = _imageFileNameGenerator.GetFilename(base64ParsingResult.Imagetype);
            await File.WriteAllBytesAsync(filename, data, cancellationToken);
            await _accountRepository.UpdateAsync(request.AccountId!.Value, request.FirstName, request.LastName, request.Email, request.PhotoUrl);
        }
    }
}

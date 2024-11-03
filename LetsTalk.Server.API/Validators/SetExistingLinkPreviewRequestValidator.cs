using FluentValidation;
using LetsTalk.Server.API.Models.Message;
using LetsTalk.Server.SignPackage.Abstractions;

namespace LetsTalk.Server.API.Core.Features.Message.Commands.CreateMessage;

public class SetExistingLinkPreviewRequestValidator : AbstractValidator<SetExistingLinkPreviewRequest>
{
    private readonly ISignPackageService _signPackageService;

    public SetExistingLinkPreviewRequestValidator(ISignPackageService signPackageService)
    {
        RuleFor(model => model)
            .Must(IsSignatureValid!)
            .WithMessage("Image signature is invalid");

        _signPackageService = signPackageService;
    }

    private bool IsSignatureValid(SetExistingLinkPreviewRequest model)
    {
        return _signPackageService.Validate(model);
    }
}

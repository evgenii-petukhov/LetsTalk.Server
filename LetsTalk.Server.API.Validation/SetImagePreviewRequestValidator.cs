using FluentValidation;
using LetsTalk.Server.API.Models.Message;
using LetsTalk.Server.SignPackage.Abstractions;

namespace LetsTalk.Server.API.Validation;

public class SetImagePreviewRequestValidator : AbstractValidator<SetImagePreviewRequest>
{
    private readonly ISignPackageService _signPackageService;

    public SetImagePreviewRequestValidator(ISignPackageService signPackageService)
    {
        RuleFor(model => model)
            .Must(IsSignatureValid!)
            .WithMessage("Image signature is invalid");

        _signPackageService = signPackageService;
    }

    private bool IsSignatureValid(SetImagePreviewRequest model)
    {
        return _signPackageService.Validate(model);
    }
}

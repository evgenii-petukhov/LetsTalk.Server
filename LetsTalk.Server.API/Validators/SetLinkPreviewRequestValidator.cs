using FluentValidation;
using LetsTalk.Server.API.Models.Message;
using LetsTalk.Server.SignPackage.Abstractions;

namespace LetsTalk.Server.API.Core.Features.Message.Commands.CreateMessage;

public class SetLinkPreviewRequestValidator : AbstractValidator<SetLinkPreviewRequest>
{
    private readonly ISignPackageService _signPackageService;

    public SetLinkPreviewRequestValidator(ISignPackageService signPackageService)
    {
        RuleFor(model => model)
            .Must(IsSignatureValid!)
            .WithMessage("Image signature is invalid");

        _signPackageService = signPackageService;
    }

    private bool IsSignatureValid(SetLinkPreviewRequest model)
    {
        return _signPackageService.Validate(model);
    }
}

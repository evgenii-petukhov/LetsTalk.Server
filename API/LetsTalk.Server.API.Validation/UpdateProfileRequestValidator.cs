using FluentValidation;
using LetsTalk.Server.API.Models.Message;
using LetsTalk.Server.API.Models.Profile;
using LetsTalk.Server.SignPackage.Abstractions;

namespace LetsTalk.Server.API.Validation;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    private readonly ISignPackageService _signPackageService;

    public UpdateProfileRequestValidator(ISignPackageService signPackageService)
    {
        RuleFor(model => model.FirstName)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty");

        RuleFor(model => model.LastName)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty");

        When(model => model.Image != null, () =>
        {
            RuleFor(model => model.Image)
                .Must(IsSignatureValid!)
                .WithMessage("Image signature is invalid");
        });

        _signPackageService = signPackageService;
    }

    private bool IsSignatureValid(ImageRequestModel model)
    {
        return _signPackageService.Validate(model);
    }
}

using FluentValidation;
using LetsTalk.Server.API.Models.Message;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.SignPackage.Abstractions;

namespace LetsTalk.Server.Core.Features.Profile.Commands.UpdateProfile;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    private readonly ISignPackageService _signPackageService;
    private readonly IAccountAgnosticService _accountAgnosticService;

    public UpdateProfileCommandValidator(
        ISignPackageService signPackageService,
        IAccountAgnosticService accountAgnosticService)
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

        RuleFor(model => model.Email)
            .EmailAddress()
            .WithMessage("{PropertyName} is invalid");

        When(model => string.IsNullOrWhiteSpace(model.Email), () =>
        {
            RuleFor(model => model)
                .MustAsync(IsEmailAllowedToBeEmptyAsync)
                .WithMessage("Email cannot be empty for this account type");
        });

        When(model => model.Image != null, () =>
        {
            RuleFor(model => model.Image)
                .Must(IsSignatureValid!)
                .WithMessage("Image signature is invalid");
        });

        _signPackageService = signPackageService;
        _accountAgnosticService = accountAgnosticService;
    }

    private bool IsSignatureValid(ImageRequestModel model)
    {
        return _signPackageService.Validate(model);
    }

    private async Task<bool> IsEmailAllowedToBeEmptyAsync(UpdateProfileCommand model, CancellationToken cancellationToken)
    {
        var account = await _accountAgnosticService.GetByIdAsync(model.AccountId!, cancellationToken);

        return account.AccountTypeId != (int)AccountTypes.Email;
    }
}

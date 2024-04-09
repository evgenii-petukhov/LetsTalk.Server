using FluentValidation;
using LetsTalk.Server.API.Models.Messages;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.SignPackage.Abstractions;

namespace LetsTalk.Server.Core.Features.Profile.Commands.UpdateProfileCommand;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    private readonly IAccountAgnosticService _accountAgnosticService;
    private readonly ISignPackageService _signPackageService;

    public UpdateProfileCommandValidator(
        IAccountAgnosticService accountAgnosticService,
        ISignPackageService signPackageService)
    {
        RuleFor(model => model.AccountId)
            .NotNull()
            .WithMessage("{PropertyName} is required");

        When(model => !string.IsNullOrWhiteSpace(model.AccountId), () =>
        {
            RuleFor(model => model.AccountId)
                .MustAsync(IsAccountIdValidAsync)
                .WithMessage("Account with {PropertyName} = {PropertyValue} does not exist");
        });

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

        When(model => model.Image != null, () =>
        {
            RuleFor(model => model.Image)
                .Must(IsSignatureValid!)
                .WithMessage("Image signature is invalid");
        });

        _accountAgnosticService = accountAgnosticService;
        _signPackageService = signPackageService;
    }

    private async Task<bool> IsAccountIdValidAsync(string? id, CancellationToken cancellationToken)
    {
        return !string.IsNullOrWhiteSpace(id)
            && await _accountAgnosticService.IsAccountIdValidAsync(id, cancellationToken);
    }

    private bool IsSignatureValid(ImageRequestModel model)
    {
        return _signPackageService.Validate(model);
    }
}

using FluentValidation;
using LetsTalk.Server.Core.Helpers;
using LetsTalk.Server.Persistence.Abstractions;

namespace LetsTalk.Server.Core.Features.Profile.Commands.UpdateProfileCommand;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    private readonly IAccountRepository _accountRepository;

    public UpdateProfileCommandValidator(IAccountRepository accountRepository)
    {
        RuleFor(model => model.AccountId)
            .NotNull()
            .WithMessage("{PropertyName} is required");

        When(model => model.AccountId.HasValue, () =>
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

        When(model => model.PhotoUrl != null, () =>
        {
            RuleFor(model => model.PhotoUrl)
                .Must(IsValidPhotoUrl)
                .WithMessage("{PropertyName} is invalid base64 string or url");
        });

        _accountRepository = accountRepository;
    }

    private Task<bool> IsAccountIdValidAsync(int? id, CancellationToken token)
    {
        return id.HasValue
            ? _accountRepository.IsAccountIdValidAsync(id.Value)
            : Task.FromResult(false);
    }

    private static bool IsValidPhotoUrl(string? input)
    {
        return IsValidUrl(input) || Base64Helper.IsBase64Image(input);
    }

    private static bool IsValidUrl(string? input)
    {
        if (input == null) return false;

        return Uri.TryCreate(input, UriKind.Absolute, out var uriResult)
            && uriResult.Scheme == Uri.UriSchemeHttps;
    }
}

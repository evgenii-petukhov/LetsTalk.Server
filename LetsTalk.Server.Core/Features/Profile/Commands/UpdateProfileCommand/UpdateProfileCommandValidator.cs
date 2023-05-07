using FluentValidation;
using LetsTalk.Server.Persistence.Abstractions;

namespace LetsTalk.Server.Core.Features.Profile.Commands.UpdateProfileCommand;

public class UpdateProfileCommandValidator: AbstractValidator<UpdateProfileCommand>
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
                .Matches("data:image\\/(jpeg|png|gif){1};base64,([^\\\"]*)")
                .WithMessage("{PropertyName} is invalid base64 string");
        });

        _accountRepository = accountRepository;
    }

    private Task<bool> IsAccountIdValidAsync(int? id, CancellationToken token)
    {
        return id.HasValue
            ? _accountRepository.IsAccountIdValidAsync(id.Value)
            : Task.FromResult(false);
    }
}

using FluentValidation;
using LetsTalk.Server.Persistence.Abstractions;

namespace LetsTalk.Server.Core.Features.Profile.Commands.UpdateProfileCommand;

public class UpdateProfileCommandValidator: AbstractValidator<UpdateProfileCommand>
{
    private readonly IAccountRepository _accountRepository;

    public UpdateProfileCommandValidator(IAccountRepository accountRepository)
    {
        RuleFor(model => model.AccountId)
            .NotNull().WithMessage("{PropertyName} is required")
            .MustAsync(IsAccountIdValidAsync).WithMessage("Account with {PropertyName} = {PropertyValue} does not exist");

        RuleFor(model => model.FirstName)
            .NotNull().WithMessage("{PropertyName} is required")
            .NotEmpty().WithMessage("{PropertyName} is required");

        RuleFor(model => model.LastName)
            .NotNull().WithMessage("{PropertyName} is required")
            .NotEmpty().WithMessage("{PropertyName} is required");

        RuleFor(model => model.Email)
            .EmailAddress();

        _accountRepository = accountRepository;
    }

    private Task<bool> IsAccountIdValidAsync(int id, CancellationToken token)
    {
        return _accountRepository.IsAccountIdValidAsync(id);
    }
}

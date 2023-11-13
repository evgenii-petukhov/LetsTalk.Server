using FluentValidation;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

namespace LetsTalk.Server.Core.Features.Account.Commands.UpdateProfileCommand;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    private readonly IAccountAgnosticService _accountAgnosticService;

    public UpdateProfileCommandValidator(IAccountAgnosticService accountAgnosticService)
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

        _accountAgnosticService = accountAgnosticService;
    }

    private async Task<bool> IsAccountIdValidAsync(int? id, CancellationToken cancellationToken)
    {
        return id.HasValue
            && await _accountAgnosticService.IsAccountIdValidAsync(id.Value, cancellationToken);
    }
}

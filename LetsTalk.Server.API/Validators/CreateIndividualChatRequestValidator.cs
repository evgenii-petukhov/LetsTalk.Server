using FluentValidation;
using LetsTalk.Server.API.Models.Chat;

namespace LetsTalk.Server.API.Validators;

public class CreateIndividualChatRequestValidator : AbstractValidator<CreateIndividualChatRequest>
{
    public CreateIndividualChatRequestValidator()
    {
        RuleFor(model => model.AccountId)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty");

        When(model => !string.IsNullOrWhiteSpace(model.AccountId) && int.TryParse(model.AccountId, out _), () =>
        {
            RuleFor(model => model.AccountId)
                .Must(accountId => int.TryParse(accountId, out var accountIdAsInt) && accountIdAsInt > 0)
                .WithMessage("{PropertyName} must be greater than 0, if integer");
        });
    }
}

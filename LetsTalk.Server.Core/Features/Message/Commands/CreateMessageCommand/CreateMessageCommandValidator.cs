using FluentValidation;
using LetsTalk.Server.Persistence.Abstractions;

namespace LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;

public class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
{
    private readonly IAccountRepository _accountRepository;

    public CreateMessageCommandValidator(IAccountRepository accountRepository)
    {
        RuleFor(model => model)
            .Must(IsContentValid)
            .WithMessage("Text and ImageId both cannot be empty");

        RuleFor(model => model.SenderId)
            .NotNull()
            .WithMessage("{PropertyName} is required");

        RuleFor(model => model.RecipientId)
            .NotNull()
            .WithMessage("{PropertyName} is required");

        When(model => model.RecipientId.HasValue, () =>
        {
            RuleFor(model => model.RecipientId)
                .NotEqual(model => model.SenderId)
                .WithMessage("{PropertyName} can't be equal to {ComparisonProperty}")
                .MustAsync(IsAccountIdValidAsync)
                .WithMessage("Account with {PropertyName} = {PropertyValue} does not exist");
        });

        _accountRepository = accountRepository;
    }

    private async Task<bool> IsAccountIdValidAsync(int? id, CancellationToken cancellationToken)
    {
        return id.HasValue
            && await _accountRepository.IsAccountIdValidAsync(id.Value, cancellationToken);
    }

    private bool IsContentValid(CreateMessageCommand model)
    {
        return !string.IsNullOrEmpty(model.Text) || model.ImageId.HasValue;
    }
}

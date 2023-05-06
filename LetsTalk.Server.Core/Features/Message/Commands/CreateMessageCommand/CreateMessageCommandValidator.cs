using FluentValidation;
using LetsTalk.Server.Persistence.Abstractions;

namespace LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;

public class CreateMessageCommandValidator: AbstractValidator<CreateMessageCommand>
{
    private readonly IAccountRepository _accountRepository;

    public CreateMessageCommandValidator(IAccountRepository accountRepository)
    {
        RuleFor(model => model.Text)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty");

        RuleFor(model => model.SenderId)
            .NotNull()
            .WithMessage("{PropertyName} is required");

        RuleFor(model => model.RecipientId)
            .NotNull()
            .WithMessage("{PropertyName} is required");

        When(x => x.RecipientId.HasValue, () =>
        {
            RuleFor(model => model.RecipientId)
                .NotEqual(model => model.SenderId)
                .WithMessage("{PropertyName} can't be eaual to {ComparisonProperty}")
                .MustAsync(IsAccountIdValidAsync)
                .WithMessage("Account with {PropertyName} = {PropertyValue} does not exist");
        });

        _accountRepository = accountRepository;
    }

    private Task<bool> IsAccountIdValidAsync(int? id, CancellationToken token)
    {
        return _accountRepository.IsAccountIdValidAsync(id!.Value);
    }
}

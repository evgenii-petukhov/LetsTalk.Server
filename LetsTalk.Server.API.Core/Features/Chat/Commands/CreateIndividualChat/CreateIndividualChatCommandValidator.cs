using FluentValidation;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

namespace LetsTalk.Server.Core.Features.Chat.Commands.CreateIndividualChat;

public class CreateIndividualChatCommandValidator : AbstractValidator<CreateIndividualChatCommand>
{
    private readonly IAccountAgnosticService _accountAgnosticService;

    public CreateIndividualChatCommandValidator(
        IAccountAgnosticService accountAgnosticService)
    {
        RuleFor(model => model.AccountId)
            .NotNull()
            .NotEmpty()
            .WithMessage("'{PropertyName}' is required")
            .Must(IsAccountIdValid!)
            .WithMessage("'{PropertyName}' must be greater than 0, when it is an integer");

        When(model => !string.IsNullOrWhiteSpace(model.AccountId) && IsAccountIdValid(model.AccountId), () =>
        {
            RuleFor(model => model.AccountId)
                .MustAsync(IsAccountIdValidAsync!)
                .WithMessage("Account with '{PropertyName}' = '{PropertyValue}' must exist");
        });

        _accountAgnosticService = accountAgnosticService;
    }

    private bool IsAccountIdValid(string accountId)
    {
        return !int.TryParse(accountId, out var accountIdAsInt) || accountIdAsInt > 0;
    }

    private async Task<bool> IsAccountIdValidAsync(string id, CancellationToken cancellationToken)
    {
        return !string.IsNullOrWhiteSpace(id)
            && await _accountAgnosticService.IsAccountIdValidAsync(id, cancellationToken);
    }
}

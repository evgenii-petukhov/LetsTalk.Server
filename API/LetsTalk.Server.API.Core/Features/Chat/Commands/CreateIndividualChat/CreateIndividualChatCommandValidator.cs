using FluentValidation;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

namespace LetsTalk.Server.API.Core.Features.Chat.Commands.CreateIndividualChat;

public class CreateIndividualChatCommandValidator : AbstractValidator<CreateIndividualChatCommand>
{
    private readonly IAccountAgnosticService _accountAgnosticService;

    public CreateIndividualChatCommandValidator(
        IAccountAgnosticService accountAgnosticService)
    {
        RuleFor(model => model.AccountId)
            .MustAsync(IsAccountIdValidAsync!)
            .WithMessage("Account with '{PropertyName}' = '{PropertyValue}' must exist");

        _accountAgnosticService = accountAgnosticService;
    }

    private async Task<bool> IsAccountIdValidAsync(string id, CancellationToken cancellationToken)
    {
        return !string.IsNullOrWhiteSpace(id)
            && await _accountAgnosticService.IsAccountIdValidAsync(id, cancellationToken);
    }
}

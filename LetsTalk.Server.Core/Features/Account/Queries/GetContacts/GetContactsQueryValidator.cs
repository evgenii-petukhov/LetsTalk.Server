using FluentValidation;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

namespace LetsTalk.Server.Core.Features.Account.Queries.GetContacts;

public class GetContactsQueryValidator : AbstractValidator<GetContactsQuery>
{
    private readonly IAccountAgnosticService _accountAgnosticService;

    public GetContactsQueryValidator(IAccountAgnosticService accountAgnosticService)
    {
        RuleFor(model => model.Id)
            .NotNull()
            .WithMessage("{PropertyName} is required");

        When(model => !string.IsNullOrWhiteSpace(model.Id), () =>
        {
            RuleFor(model => model.Id)
                .MustAsync(IsAccountIdValidAsync)
                .WithMessage("Account with {PropertyName} = {PropertyValue} does not exist");
        });

        _accountAgnosticService = accountAgnosticService;
    }

    private async Task<bool> IsAccountIdValidAsync(string? id, CancellationToken cancellationToken)
    {
        return !string.IsNullOrWhiteSpace(id)
            && await _accountAgnosticService.IsAccountIdValidAsync(id, cancellationToken);
    }
}

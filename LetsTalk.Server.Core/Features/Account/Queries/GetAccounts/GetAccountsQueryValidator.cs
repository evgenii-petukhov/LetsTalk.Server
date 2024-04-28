using FluentValidation;

namespace LetsTalk.Server.Core.Features.Account.Queries.GetAccounts;

public class GetAccountsQueryValidator : AbstractValidator<GetAccountsQuery>
{
    public GetAccountsQueryValidator()
    {
        RuleFor(model => model.Id)
            .NotNull()
            .NotEmpty()
            .WithMessage("{PropertyName} is required");
    }
}

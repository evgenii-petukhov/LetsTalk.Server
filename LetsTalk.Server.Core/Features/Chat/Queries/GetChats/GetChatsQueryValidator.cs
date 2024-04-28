using FluentValidation;

namespace LetsTalk.Server.Core.Features.Chat.Queries.GetChats;

public class GetChatsQueryValidator : AbstractValidator<GetChatsQuery>
{
    public GetChatsQueryValidator()
    {
        RuleFor(model => model.Id)
            .NotNull()
            .NotEmpty()
            .WithMessage("{PropertyName} is required");
    }
}

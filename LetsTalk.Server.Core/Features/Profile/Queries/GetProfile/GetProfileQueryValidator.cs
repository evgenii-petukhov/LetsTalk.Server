using FluentValidation;

namespace LetsTalk.Server.Core.Features.Profile.Queries.GetProfile;

public class GetProfileQueryValidator : AbstractValidator<GetProfileQuery>
{
    public GetProfileQueryValidator()
    {
        RuleFor(model => model.Id)
            .NotNull()
            .NotEmpty()
            .WithMessage("{PropertyName} is required");
    }
}

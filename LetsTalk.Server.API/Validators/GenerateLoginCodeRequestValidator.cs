using FluentValidation;
using LetsTalk.Server.API.Models.Login;
using LetsTalk.Server.Utility.Common;

namespace LetsTalk.Server.API.Validators;

public class GenerateLoginCodeRequestValidator : AbstractValidator<GenerateLoginCodeRequest>
{
    public GenerateLoginCodeRequestValidator()
    {
        RuleFor(model => model.Email)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty")
            .EmailAddress()
            .WithMessage("{PropertyName} must be a valid email");

        RuleFor(model => model.AntiSpamToken)
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty")
            .Must(IsUnixTimeValid)
            .WithMessage("Failed the anti-spam check");
    }

    private static bool IsUnixTimeValid(long unixTime)
    {
        var dt = DateHelper.GetUnixTimestamp();
        return Math.Abs(dt - unixTime) < 60;
    }
}

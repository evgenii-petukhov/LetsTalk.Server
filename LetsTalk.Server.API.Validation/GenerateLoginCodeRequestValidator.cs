using FluentValidation;
using LetsTalk.Server.API.Models.Login;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Utility.Common;

namespace LetsTalk.Server.API.Validation;

public class GenerateLoginCodeRequestValidator : AbstractValidator<GenerateLoginCodeRequest>
{
    private readonly SecuritySettings _securitySettings;

    public GenerateLoginCodeRequestValidator(SecuritySettings securitySettings)
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
            .WithMessage("Anti-spam check failed");

        _securitySettings = securitySettings;
    }

    private bool IsUnixTimeValid(long unixTime)
    {
        var dt = DateHelper.GetUnixTimestamp();
        return Math.Abs(dt - unixTime) < _securitySettings.AntiSpamTokenLifeTimeInSeconds;
    }
}

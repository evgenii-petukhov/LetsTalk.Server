using FluentValidation;
using LetsTalk.Server.API.Models.Login;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Utility.Common;

namespace LetsTalk.Server.API.Validation;

public class EmailLoginRequestValidator : AbstractValidator<EmailLoginRequest>
{
    private readonly SecuritySettings _securitySettings;

    public EmailLoginRequestValidator(SecuritySettings securitySettings)
    {
        RuleFor(model => model.Email)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty")
            .EmailAddress()
            .WithMessage("{PropertyName} must be a valid email");

        RuleFor(model => model.Code)
            .InclusiveBetween(1000, 9999)
            .WithMessage("Code should contain 4 digits");

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

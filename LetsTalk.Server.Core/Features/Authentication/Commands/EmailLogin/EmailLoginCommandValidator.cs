using FluentValidation;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Core.Features.Authentication.Commands.EmailLogin;

namespace LetsTalk.Server.Core.Features.Profile.Commands.UpdateProfile;

public class EmailLoginCommandValidator : AbstractValidator<EmailLoginCommand>
{
    private readonly ILoginCodeCacheService _loginCodeCacheService;

    public EmailLoginCommandValidator(ILoginCodeCacheService loginCodeCacheService)
    {
        RuleFor(model => model.Email)
            .NotNull()
            .WithMessage("'{PropertyName}' is required")
            .NotEmpty()
            .WithMessage("'{PropertyName}' cannot be empty")
            .EmailAddress()
            .WithMessage("'{PropertyName}' must be a valid email");

        RuleFor(model => model.Code)
            .InclusiveBetween(1000, 9999)
            .WithMessage("'Code' should contain 4 digits");

        RuleFor(model => model)
            .MustAsync(IsLoginCodeValidAsync)
            .WithMessage("'Code' has expired");

        _loginCodeCacheService = loginCodeCacheService;
    }

    private Task<bool> IsLoginCodeValidAsync(EmailLoginCommand model, CancellationToken cancellationToken)
    {
        return _loginCodeCacheService.ValidateCodeAsync(model.Email!, model.Code);
    }
}

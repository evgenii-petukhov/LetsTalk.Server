using FluentValidation;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Commands;

namespace LetsTalk.Server.API.Core.Features.Authentication.Commands.EmailLogin;

public class EmailLoginCommandValidator : AbstractValidator<EmailLoginCommand>
{
    private readonly ILoginCodeCacheService _loginCodeCacheService;

    public EmailLoginCommandValidator(ILoginCodeCacheService loginCodeCacheService)
    {
        RuleFor(model => model)
            .MustAsync(IsLoginCodeValidAsync)
            .WithMessage("Code has expired");

        _loginCodeCacheService = loginCodeCacheService;
    }

    private Task<bool> IsLoginCodeValidAsync(EmailLoginCommand model, CancellationToken cancellationToken)
    {
        return _loginCodeCacheService.ValidateCodeAsync(model.Email?.Trim().ToLowerInvariant()!, model.Code);
    }
}

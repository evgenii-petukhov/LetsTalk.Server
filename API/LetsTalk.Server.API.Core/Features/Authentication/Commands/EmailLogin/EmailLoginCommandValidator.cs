using FluentValidation;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.Authentication.Abstractions;

namespace LetsTalk.Server.API.Core.Features.Authentication.Commands.EmailLogin;

public class EmailLoginCommandValidator : AbstractValidator<EmailLoginCommand>
{
    private readonly IAuthenticationClient _authenticationClient;

    public EmailLoginCommandValidator(IAuthenticationClient authenticationClient)
    {
        RuleFor(model => model)
            .MustAsync(IsLoginCodeValidAsync)
            .WithMessage("Code has expired");

        _authenticationClient = authenticationClient;
    }

    private Task<bool> IsLoginCodeValidAsync(EmailLoginCommand model, CancellationToken cancellationToken)
    {
        return _authenticationClient.ValidateLoginCodeAsync(model.Email?.Trim().ToLowerInvariant()!, model.Code);
    }
}

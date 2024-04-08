using FluentValidation;
using LetsTalk.Server.API.Models.Messages;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.SignPackage.Abstractions;

namespace LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;

public class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
{
    private readonly IAccountAgnosticService _accountAgnosticService;
    private readonly ISignPackageService _signPackageService;

    public CreateMessageCommandValidator(
        IAccountAgnosticService accountAgnosticService,
        ISignPackageService signPackageService)
    {
        RuleFor(model => model)
            .Must(IsContentValid)
            .WithMessage("Text and ImageId both cannot be empty");

        RuleFor(model => model.SenderId)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .MustAsync(IsAccountIdValidAsync!)
            .WithMessage("Account with {PropertyName} = {PropertyValue} does not exist");

        RuleFor(model => model.ChatId)
            .NotNull()
            .WithMessage("{PropertyName} is required");

        When(model => model.Image != null, () =>
        {
            RuleFor(model => model.Image)
                .Must(IsSignatureValid!)
                .WithMessage("Image signature is invalid");
        });

        _accountAgnosticService = accountAgnosticService;
        _signPackageService = signPackageService;
    }

    private async Task<bool> IsAccountIdValidAsync(string id, CancellationToken cancellationToken)
    {
        return !string.IsNullOrWhiteSpace(id)
            && await _accountAgnosticService.IsAccountIdValidAsync(id, cancellationToken);
    }

    private bool IsContentValid(CreateMessageCommand model)
    {
        return !string.IsNullOrWhiteSpace(model.Text) || model.Image != null;
    }

    private bool IsSignatureValid(ImageRequestModel model)
    {
        return _signPackageService.Validate(model);
    }
}

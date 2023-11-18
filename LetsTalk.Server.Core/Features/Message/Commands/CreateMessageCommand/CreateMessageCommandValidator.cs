using FluentValidation;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

namespace LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;

public class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
{
    private readonly IAccountAgnosticService _accountAgnosticService;
    private readonly IImageAgnosticService _imageAgnosticService;

    public CreateMessageCommandValidator(
        IAccountAgnosticService accountAgnosticService,
        IImageAgnosticService imageAgnosticService)
    {
        RuleFor(model => model)
            .Must(IsContentValid)
            .WithMessage("Text and ImageId both cannot be empty");

        RuleFor(model => model.SenderId)
            .NotNull()
            .WithMessage("{PropertyName} is required");

        RuleFor(model => model.RecipientId)
            .NotNull()
            .WithMessage("{PropertyName} is required");

        When(model => !string.IsNullOrWhiteSpace(model.RecipientId), () =>
        {
            RuleFor(model => model.RecipientId)
                .NotEqual(model => model.SenderId)
                .WithMessage("{PropertyName} can't be equal to {ComparisonProperty}")
                .MustAsync(IsAccountIdValidAsync)
                .WithMessage("Account with {PropertyName} = {PropertyValue} does not exist");
        });

        When(model => !string.IsNullOrWhiteSpace(model.ImageId), () =>
        {
            RuleFor(model => model.ImageId)
                .NotEqual("0")
                .WithMessage("{PropertyName} can't be zero")
                .MustAsync(IsImageIdValidAsync)
                .WithMessage("Image with {PropertyName} = {PropertyValue} does not exist");
        });

        _accountAgnosticService = accountAgnosticService;
        _imageAgnosticService = imageAgnosticService;
    }

    private async Task<bool> IsAccountIdValidAsync(string id, CancellationToken cancellationToken)
    {
        return !string.IsNullOrWhiteSpace(id)
            && await _accountAgnosticService.IsAccountIdValidAsync(id, cancellationToken);
    }

    private async Task<bool> IsImageIdValidAsync(string id, CancellationToken cancellationToken)
    {
        return !string.IsNullOrWhiteSpace(id)
            && await _imageAgnosticService.IsImageIdValidAsync(id, cancellationToken);
    }

    private bool IsContentValid(CreateMessageCommand model)
    {
        return !string.IsNullOrWhiteSpace(model.Text) || !string.IsNullOrWhiteSpace(model.ImageId);
    }
}

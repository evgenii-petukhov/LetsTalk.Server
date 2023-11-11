using FluentValidation;
using LetsTalk.Server.Persistence.DatabaseAgnosticServices.Abstractions;

namespace LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;

public class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
{
    private readonly IAccountDatabaseAgnosticService _accountDatabaseAgnosticService;
    private readonly IImageDatabaseAgnosticService _imageDatabaseAgnosticService;

    public CreateMessageCommandValidator(
        IAccountDatabaseAgnosticService accountDatabaseAgnosticService,
        IImageDatabaseAgnosticService imageDatabaseAgnosticService)
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

        When(model => model.RecipientId.HasValue, () =>
        {
            RuleFor(model => model.RecipientId)
                .NotEqual(model => model.SenderId)
                .WithMessage("{PropertyName} can't be equal to {ComparisonProperty}")
                .MustAsync(IsAccountIdValidAsync)
                .WithMessage("Account with {PropertyName} = {PropertyValue} does not exist");
        });

        When(model => model.ImageId.HasValue, () =>
        {
            RuleFor(model => model.ImageId)
                .GreaterThan(0)
                .WithMessage("{PropertyName} can't be zero")
                .MustAsync(IsImageIdValidAsync)
                .WithMessage("Image with {PropertyName} = {PropertyValue} does not exist");
        });

        _accountDatabaseAgnosticService = accountDatabaseAgnosticService;
        _imageDatabaseAgnosticService = imageDatabaseAgnosticService;
    }

    private async Task<bool> IsAccountIdValidAsync(int? id, CancellationToken cancellationToken)
    {
        return id.HasValue
            && await _accountDatabaseAgnosticService.IsAccountIdValidAsync(id.Value, cancellationToken);
    }

    private async Task<bool> IsImageIdValidAsync(int? id, CancellationToken cancellationToken)
    {
        return id.HasValue
            && await _imageDatabaseAgnosticService.IsImageIdValidAsync(id.Value, cancellationToken);
    }

    private bool IsContentValid(CreateMessageCommand model)
    {
        return !string.IsNullOrEmpty(model.Text) || model.ImageId.HasValue;
    }
}

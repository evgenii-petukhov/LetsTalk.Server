using FluentValidation;
using LetsTalk.Server.Persistence.Abstractions;

namespace LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;

public class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
{
    private readonly IAccountDataLayerService _accountDataLayerService;

    public CreateMessageCommandValidator(IAccountDataLayerService accountDataLayerService)
    {
        RuleFor(model => model)
            .Must(IsContentValid)
            .WithMessage("{PropertyName} is required")
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty");

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

        _accountDataLayerService = accountDataLayerService;
    }

    private async Task<bool> IsAccountIdValidAsync(int? id, CancellationToken cancellationToken)
    {
        return id.HasValue
            && await _accountDataLayerService.IsAccountIdValidAsync(id.Value, cancellationToken: cancellationToken);
    }

    private bool IsContentValid(CreateMessageCommand model)
    {
        return !string.IsNullOrEmpty(model.Text) || model.ImageId.HasValue;
    }
}

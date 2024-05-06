using FluentValidation;
using LetsTalk.Server.API.Models.Message;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.SignPackage.Abstractions;

namespace LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;

public class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
{
    private readonly ISignPackageService _signPackageService;
    private readonly IChatAgnosticService _chatAgnosticService;

    public CreateMessageCommandValidator(
        ISignPackageService signPackageService,
        IChatAgnosticService chatAgnosticService)
    {
        RuleFor(model => model.ChatId)
            .NotNull()
            .NotEmpty()
            .WithMessage("'{PropertyName}' is required")
            .Must(IsChatIdValid!)
            .WithMessage("'{PropertyName}' must be greater than 0, when it is an integer");

        When(model => !string.IsNullOrWhiteSpace(model.ChatId) && IsChatIdValid(model.ChatId), () =>
        {
            RuleFor(model => model.ChatId)
                .MustAsync(IsChatIdValidAsync!)
                .WithMessage("Chat with '{PropertyName}' = '{PropertyValue}' must exist");
        });

        RuleFor(model => model)
            .Must(IsContentValid)
            .WithMessage("'Text' and 'ImageId' both cannot be empty");

        When(model => model.Image != null, () =>
        {
            RuleFor(model => model.Image)
                .Must(IsSignatureValid!)
                .WithMessage("'Image' signature is invalid");
        });

        _signPackageService = signPackageService;
        _chatAgnosticService = chatAgnosticService;
    }

    private bool IsContentValid(CreateMessageCommand model)
    {
        return !string.IsNullOrWhiteSpace(model.Text) || model.Image != null;
    }

    private bool IsSignatureValid(ImageRequestModel model)
    {
        return _signPackageService.Validate(model);
    }

    private bool IsChatIdValid(string chatId)
    {
        return !int.TryParse(chatId, out var chatIdAsInt) || chatIdAsInt > 0;
    }

    private async Task<bool> IsChatIdValidAsync(string id, CancellationToken cancellationToken)
    {
        return !string.IsNullOrWhiteSpace(id)
            && await _chatAgnosticService.IsChatIdValidAsync(id, cancellationToken);
    }
}

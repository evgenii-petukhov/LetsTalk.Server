using FluentValidation;
using LetsTalk.Server.API.Models.Message;
using LetsTalk.Server.SignPackage.Abstractions;

namespace LetsTalk.Server.API.Core.Features.Message.Commands.CreateMessage;

public class CreateMessageRequestValidator : AbstractValidator<CreateMessageRequest>
{
    private readonly ISignPackageService _signPackageService;

    public CreateMessageRequestValidator(ISignPackageService signPackageService)
    {
        RuleFor(model => model.ChatId)
            .NotNull()
            .WithMessage("{PropertyName} is required")
            .NotEmpty()
            .WithMessage("{PropertyName} cannot be empty");

        When(model => !string.IsNullOrWhiteSpace(model.ChatId) && int.TryParse(model.ChatId, out _), () =>
        {
            RuleFor(model => model.ChatId)
                .Must(chatId => int.TryParse(chatId, out var chatIdAsInt) && chatIdAsInt > 0)
                .WithMessage("{PropertyName} must be greater than 0, if integer");
        });

        RuleFor(model => model)
            .Must(IsContentValid)
            .WithMessage("Text and ImageId both cannot be empty");

        When(model => model.Image != null, () =>
        {
            RuleFor(model => model.Image)
                .Must(IsSignatureValid!)
                .WithMessage("Image signature is invalid");
        });

        _signPackageService = signPackageService;
    }

    private bool IsContentValid(CreateMessageRequest model)
    {
        return !string.IsNullOrWhiteSpace(model.Text) || model.Image != null;
    }

    private bool IsSignatureValid(ImageRequestModel model)
    {
        return _signPackageService.Validate(model);
    }
}

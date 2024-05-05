using FluentValidation;
using LetsTalk.Server.API.Models.Message;
using LetsTalk.Server.SignPackage.Abstractions;

namespace LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;

public class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
{
    private readonly ISignPackageService _signPackageService;

    public CreateMessageCommandValidator(
        ISignPackageService signPackageService)
    {
        RuleFor(model => model)
            .Must(IsContentValid)
            .WithMessage("Text and ImageId both cannot be empty");

        RuleFor(model => model.SenderId)
            .NotNull()
            .NotEmpty()
            .WithMessage("{PropertyName} is required");

        RuleFor(model => model.ChatId)
            .NotNull()
            .WithMessage("{PropertyName} is required");

        When(model => model.Image != null, () =>
        {
            RuleFor(model => model.Image)
                .Must(IsSignatureValid!)
                .WithMessage("Image signature is invalid");
        });

        _signPackageService = signPackageService;
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

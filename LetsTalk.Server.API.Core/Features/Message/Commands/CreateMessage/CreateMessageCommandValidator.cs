using FluentValidation;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

namespace LetsTalk.Server.API.Core.Features.Message.Commands.CreateMessage;

public class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommand>
{
    private readonly IChatAgnosticService _chatAgnosticService;

    public CreateMessageCommandValidator(IChatAgnosticService chatAgnosticService)
    {
        RuleFor(model => model.ChatId)
            .MustAsync(IsChatIdValidAsync!)
            .WithMessage("Chat must exist");

        _chatAgnosticService = chatAgnosticService;
    }

    private async Task<bool> IsChatIdValidAsync(string id, CancellationToken cancellationToken)
    {
        return !string.IsNullOrWhiteSpace(id)
            && await _chatAgnosticService.IsChatIdValidAsync(id, cancellationToken);
    }
}

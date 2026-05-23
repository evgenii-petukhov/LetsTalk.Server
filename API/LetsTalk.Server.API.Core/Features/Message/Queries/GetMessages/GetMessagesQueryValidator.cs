using FluentValidation;
using LetsTalk.Server.API.Core.Abstractions;

namespace LetsTalk.Server.API.Core.Features.Message.Queries.GetMessages;

public class GetMessagesQueryValidator : AbstractValidator<GetMessagesQuery>
{
    private readonly IChatService _chatService;

    public GetMessagesQueryValidator(IChatService chatService)
    {
        RuleFor(model => model.ChatId)
            .MustAsync(IsChatIdValidAsync!);

        RuleFor(model => model)
            .MustAsync(IsAccountChatMemberAsync!);

        _chatService = chatService;
    }

    private async Task<bool> IsChatIdValidAsync(string chatId, CancellationToken cancellationToken)
    {
        return !string.IsNullOrWhiteSpace(chatId)
            && await _chatService.IsChatIdValidAsync(chatId, cancellationToken);
    }

    private Task<bool> IsAccountChatMemberAsync(GetMessagesQuery model, CancellationToken cancellationToken)
    {
        return _chatService.IsAccountChatMemberAsync(model.ChatId, model.SenderId, cancellationToken);
    }
}

using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.Message.Commands.ReadMessage;

public class ReadMessageCommandHandler(
    IMessageAgnosticService messageAgnosticService,
    IChatCacheManager chatCacheManager) : IRequestHandler<ReadMessageCommand, Unit>
{
    private readonly IMessageAgnosticService _messageAgnosticService = messageAgnosticService;
    private readonly IChatCacheManager _chatCacheManager = chatCacheManager;

    public async Task<Unit> Handle(ReadMessageCommand request, CancellationToken cancellationToken)
    {
        await _messageAgnosticService.MarkAsReadAsync(
            request.ChatId!,
            request.AccountId!,
            request.MessageId!,
            cancellationToken);

        await _chatCacheManager.ClearAsync(request.AccountId!);

        return Unit.Value;
    }
}

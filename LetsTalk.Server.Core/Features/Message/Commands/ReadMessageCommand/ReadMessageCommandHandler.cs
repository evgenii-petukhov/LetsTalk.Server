using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Commands.ReadMessageCommand;

public class ReadMessageCommandHandler(IMessageAgnosticService messageAgnosticService) : IRequestHandler<ReadMessageCommand, Unit>
{
    private readonly IMessageAgnosticService _messageAgnosticService = messageAgnosticService;

    public async Task<Unit> Handle(ReadMessageCommand request, CancellationToken cancellationToken)
    {
        await _messageAgnosticService.MarkAsReadAsync(
            request.MessageId!,
            request.AccountId!,
            cancellationToken);

        return Unit.Value;
    }
}

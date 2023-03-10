using LetsTalk.Server.Abstractions.Repositories;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Commands.ReadMessageCommand;

public class ReadMessageCommandHandler : IRequestHandler<ReadMessageCommand, Unit>
{
    private readonly IMessageRepository _messageRepository;

    public ReadMessageCommandHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<Unit> Handle(ReadMessageCommand request, CancellationToken cancellationToken)
    {
        await _messageRepository.MarkAsReadAsync(request.MessageId, request.RecipientId);
        return Unit.Value;
    }
}

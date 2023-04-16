using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Queries.GetMessages;

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, MessageDto[]>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMessageProcessor _messageProcessor;

    public GetMessagesQueryHandler(
        IMessageRepository messageRepository,
        IMessageProcessor messageProcessor)
    {
        _messageRepository = messageRepository;
        _messageProcessor = messageProcessor;
    }

    public async Task<MessageDto[]> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageRepository.GetAsync(request.SenderId, request.RecipientId);

        return await Task.WhenAll(messages
            .Select(message => _messageProcessor.GetMessageDto(message, request.SenderId)));
    }
}

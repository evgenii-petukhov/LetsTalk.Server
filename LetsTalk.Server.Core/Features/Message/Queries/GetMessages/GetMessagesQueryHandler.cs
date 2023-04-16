using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Queries.GetMessages;

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, List<MessageDto>>
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

    public async Task<List<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageRepository.GetAsync(request.SenderId, request.RecipientId);

        var messagesToProcess = messages
            .Where(message => message.TextHtml == null)
            .ToList();

        Parallel.ForEach(messagesToProcess, _messageProcessor.SetTextHtml);

        foreach (var message in messagesToProcess)
        {
            await _messageRepository.SetTextHtmlAsync(message.Id, message.TextHtml!);
        }

        return messages
            .Select(message => _messageProcessor.GetMessageDto(message, request.SenderId))
            .ToList();
    }
}

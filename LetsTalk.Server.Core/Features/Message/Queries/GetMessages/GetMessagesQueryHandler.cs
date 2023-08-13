using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Queries.GetMessages;

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, List<MessageDto>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMessageProcessor _messageProcessor;
    private readonly IMapper _mapper;

    public GetMessagesQueryHandler(
        IMessageRepository messageRepository,
        IMessageProcessor messageProcessor,
        IMapper mapper)
    {
        _messageRepository = messageRepository;
        _messageProcessor = messageProcessor;
        _mapper = mapper;
    }

    public async Task<List<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageRepository.GetAsync(request.SenderId, request.RecipientId, request.PageIndex, request.MessagesPerPage, cancellationToken);

        if (messages.Any(message => !message.IsRead))
        {
            await _messageRepository.MarkAllAsReadAsync(request.SenderId, request.RecipientId, cancellationToken);
        }

        var messagesToProcess = messages
            .Where(message => message.TextHtml == null && !message.ImageId.HasValue)
            .ToList();

        if (messagesToProcess.Any())
        {
            Parallel.ForEach(messagesToProcess, message => _messageProcessor.SetTextHtml(message, out _));
            await _messageRepository.SetTextHtmlAsync(messagesToProcess, cancellationToken);
        }

        return _mapper.Map<List<MessageDto>>(messages)
            .ConvertAll(messageDto =>
            {
                messageDto.IsMine = messageDto.SenderId == request.SenderId;
                return messageDto;
            });
    }
}

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
        var messages = await _messageRepository.GetAsync(request.SenderId, request.RecipientId);

        var messagesToProcess = messages
            .Where(message => message.TextHtml == null)
            .ToList();

        Parallel.ForEach(messagesToProcess, message => _messageProcessor.SetTextHtml(message, out _));

        foreach (var message in messagesToProcess)
        {
            await _messageRepository.SetTextHtmlAsync(message.Id, message.TextHtml!);
        }

        return _mapper.Map<List<MessageDto>>(messages)
            .ConvertAll(messageDto =>
            {
                messageDto.IsMine = messageDto.AccountId == request.SenderId;
                return messageDto;
            });
    }
}

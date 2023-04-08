using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Queries.GetMessages;

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, List<MessageDto>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;
    private readonly IMessageProcessor _messageProcessor;

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
        var messageDtos = messages
            .Select(async message =>
            {
                var messageDto = _mapper.Map<MessageDto>(message);
                messageDto.IsMine = message.SenderId == request.SenderId;
                if (messageDto.TextHtml == null)
                {
                    var result = _messageProcessor.ConvertToHtml(message.Text!);
                    messageDto.TextHtml = result.Html;
                    await _messageRepository.SetTextHtmlAsync(message.Id, messageDto.TextHtml!);
                }                
                return messageDto;
            })
            .Select(async t => await t)
            .Select(t => t.Result)
            .ToList();

        return messageDtos;
    }
}

using AutoMapper;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.Message.Queries.GetMessages;

public class GetMessagesQueryHandler(
    IMapper mapper,
    IMessageService messageService) : IRequestHandler<GetMessagesQuery, List<MessageDto>>
{
    private readonly IMapper _mapper = mapper;
    private readonly IMessageService _messageService = messageService;

    public async Task<List<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageService.GetPagedAsync(
            request.ChatId,
            request.PageIndex,
            request.MessagesPerPage,
            cancellationToken);

        return messages
            .Select(message =>
            {
                var messageDto = _mapper.Map<MessageDto>(message);
                messageDto.IsMine = message.SenderId == request.SenderId;
                return messageDto;
            })
            .ToList();
    }
}

using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Queries.GetMessages;

public class GetMessagesQueryHandler(
    IMapper mapper,
    IMessageService messageService) : IRequestHandler<GetMessagesQuery, List<MessageDto>>
{
    private readonly IMapper _mapper = mapper;
    private readonly IMessageService _messageService = messageService;

    public async Task<List<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var messageDtos = await _messageService.GetPagedAsync(
            request.SenderId,
            request.RecipientId,
            request.PageIndex,
            request.MessagesPerPage,
            cancellationToken);

        return _mapper.Map<List<MessageDto>>(messageDtos);
    }
}

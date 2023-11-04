using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Queries.GetMessages;

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, List<MessageDto>>
{
    private readonly IMapper _mapper;
    private readonly IMessageService _messageService;

    public GetMessagesQueryHandler(
        IMapper mapper,
        IMessageService messageService)
    {
        _mapper = mapper;
        _messageService = messageService;
    }

    public async Task<List<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var messageCacheEntries = await _messageService.GetPagedAsync(
            request.SenderId,
            request.RecipientId,
            request.PageIndex,
            request.MessagesPerPage,
            cancellationToken);

        return _mapper.Map<List<MessageDto>>(messageCacheEntries);
    }
}

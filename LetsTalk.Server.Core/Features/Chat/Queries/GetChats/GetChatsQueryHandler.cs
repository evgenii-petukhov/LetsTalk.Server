using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.Core.Features.Chat.Queries.GetChats;

public class GetChatsQueryHandler(
    IMapper mapper,
    IChatService chatService) : IRequestHandler<GetChatsQuery, List<ChatDto>>
{
    private readonly IMapper _mapper = mapper;
    private readonly IChatService _chatService = chatService;

    public async Task<List<ChatDto>> Handle(GetChatsQuery request, CancellationToken cancellationToken)
    {
        var chatCacheEntries = await _chatService.GetChatsAsync(request.Id, cancellationToken);

        return _mapper.Map<List<ChatDto>>(chatCacheEntries);
    }
}

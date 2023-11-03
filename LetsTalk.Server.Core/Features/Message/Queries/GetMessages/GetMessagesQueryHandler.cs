using AutoMapper;
using LetsTalk.Server.Caching.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Queries.GetMessages;

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, List<MessageDto>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _messageCacheService;

    public GetMessagesQueryHandler(
        IMessageRepository messageRepository,
        IMapper mapper,
        ICacheService messageCacheService)
    {
        _messageRepository = messageRepository;
        _mapper = mapper;
        _messageCacheService = messageCacheService;
    }

    public Task<List<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        return _messageCacheService.GetOrAddMessagesAsync(request.SenderId, request.RecipientId, request.PageIndex, async () =>
        {
            var messages = await _messageRepository.GetPagedAsync(request.SenderId, request.RecipientId, request.PageIndex, request.MessagesPerPage, cancellationToken);
            return _mapper.Map<List<MessageDto>>(messages)
                .ConvertAll(messageDto =>
                {
                    messageDto.IsMine = messageDto.SenderId == request.SenderId;
                    return messageDto;
                });
        })!;
    }
}

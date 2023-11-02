using AutoMapper;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Core.Models.Caching;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace LetsTalk.Server.Core.Features.Message.Queries.GetMessages;

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, List<MessageDto>>
{
    private readonly TimeSpan CacheExpirationIntervalInSeconds;

    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;

    public GetMessagesQueryHandler(
        IMessageRepository messageRepository,
        IMapper mapper,
        IMemoryCache memoryCache,
        IOptions<MessagingSettings> messagingSettings)
    {
        _messageRepository = messageRepository;
        _mapper = mapper;
        _memoryCache = memoryCache;

        CacheExpirationIntervalInSeconds = TimeSpan.FromSeconds(messagingSettings.Value.CacheExpirationIntervalInSeconds);
    }

    public Task<List<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = _mapper.Map<MessageCacheKey>(request);

        return _memoryCache.GetOrCreateAsync(cacheKey, async cacheEntry =>
        {
            cacheEntry.SlidingExpiration = CacheExpirationIntervalInSeconds;
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

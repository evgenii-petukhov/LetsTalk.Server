using AutoMapper;
using LetsTalk.Server.Abstractions.Repositories;
using LetsTalk.Server.Models.Message;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Queries.GetMessages;

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, List<MessageDto>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public GetMessagesQueryHandler(
        IMessageRepository messageRepository,
        IMapper mapper)
    {
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    public async Task<List<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageRepository.GetBySenderAndRecipientIdsAsync(request.SenderId, request.RecipientId);
        return _mapper.Map<List<MessageDto>>(messages);
    }
}

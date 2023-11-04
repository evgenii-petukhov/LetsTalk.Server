using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.Core.Services;

public class MessageService: IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public MessageService(
        IMessageRepository messageRepository,
        IMapper mapper)
    {
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    public async Task<List<MessageDto>> GetPagedAsync(int senderId, int recipientId, int pageIndex, int messagesPerPage, CancellationToken cancellationToken)
    {
        var messages = await _messageRepository.GetPagedAsync(senderId, recipientId, pageIndex, messagesPerPage, cancellationToken);
        return _mapper.Map<List<MessageDto>>(messages)
        .ConvertAll(messageDto =>
        {
            messageDto.IsMine = messageDto.SenderId == senderId;
            return messageDto;
        });
    }
}

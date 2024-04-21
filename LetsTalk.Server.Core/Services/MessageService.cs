using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;

namespace LetsTalk.Server.Core.Services;

public class MessageService(
    IMessageAgnosticService messageAgnosticService,
    IMapper mapper) : IMessageService
{
    private readonly IMessageAgnosticService _messageAgnosticService = messageAgnosticService;
    private readonly IMapper _mapper = mapper;

    public async Task<List<MessageDto>> GetPagedAsync(string senderId, string chatId, int pageIndex, int messagesPerPage, CancellationToken cancellationToken)
    {
        var messages = await _messageAgnosticService.GetPagedAsync(chatId, pageIndex, messagesPerPage, cancellationToken);

        return messages
            .ConvertAll(message =>
            {
                var messageDto = _mapper.Map<MessageDto>(message);
                messageDto.IsMine = message.SenderId == senderId;
                return messageDto;
            });
    }
}

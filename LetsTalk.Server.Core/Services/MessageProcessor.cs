using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.Abstractions;

namespace LetsTalk.Server.Core.Services;

public class MessageProcessor : IMessageProcessor
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;
    private readonly IHtmlGenerator _htmlGenerator;

    public MessageProcessor(
        IMessageRepository messageRepository,
        IHtmlGenerator htmlGenerator,
        IMapper mapper)
    {
        _messageRepository = messageRepository;
        _htmlGenerator = htmlGenerator;
        _mapper = mapper;
    }

    public async Task<MessageDto> GetMessageDto(Domain.Message message, int senderId)
    {
        var messageDto = _mapper.Map<MessageDto>(message);
        messageDto.IsMine = message.SenderId == senderId;
        if (messageDto.TextHtml == null)
        {
            var result = _htmlGenerator.GetHtml(message.Text!);
            messageDto.TextHtml = result.Html;
            await _messageRepository.SetTextHtmlAsync(message.Id, messageDto.TextHtml!)
                .ConfigureAwait(false);
        }
        return messageDto;
    }
}

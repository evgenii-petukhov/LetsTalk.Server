using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Dto.Models;

namespace LetsTalk.Server.Core.Services;

public class MessageProcessor : IMessageProcessor
{
    private readonly IMapper _mapper;
    private readonly IHtmlGenerator _htmlGenerator;

    public MessageProcessor(
        IHtmlGenerator htmlGenerator,
        IMapper mapper)
    {
        _htmlGenerator = htmlGenerator;
        _mapper = mapper;
    }

    public MessageDto GetMessageDto(Message message, int senderId)
    {
        var messageDto = _mapper.Map<MessageDto>(message);
        messageDto.IsMine = message.SenderId == senderId;
        return messageDto;
    }

    public void SetTextHtml(Message message)
    {
        var result = _htmlGenerator.GetHtml(message.Text!);
        message.TextHtml = result.Html;
    }
}

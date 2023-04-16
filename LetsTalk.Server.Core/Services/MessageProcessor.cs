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
        return _mapper.Map<Message, MessageDto>(
            message,
            x => x.AfterMap((source, destin) => destin.IsMine = source.SenderId == senderId));
    }

    public void SetTextHtml(Message message)
    {
        var result = _htmlGenerator.GetHtml(message.Text!);
        message.TextHtml = result.Html;
    }
}

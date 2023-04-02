using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;

public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, MessageDto>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;
    private readonly IMessageProcessor _messageProcessor;

    public CreateMessageCommandHandler(
        IMessageRepository messageRepository,
        IMapper mapper,
        IMessageProcessor messageProcessor)
    {
        _messageRepository = messageRepository;
        _mapper = mapper;
        _messageProcessor = messageProcessor;
    }

    public async Task<MessageDto> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        var messageEntity = _mapper.Map<Domain.Message>(request);
        messageEntity.TextHtml = await _messageProcessor.GetHtml(messageEntity.Text!);
        await _messageRepository.CreateAsync(messageEntity);
        var messageDto = _mapper.Map<MessageDto>(messageEntity);
        return messageDto;
    }
}

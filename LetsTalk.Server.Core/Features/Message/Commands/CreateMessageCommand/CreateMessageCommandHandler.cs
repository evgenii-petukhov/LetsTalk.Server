using AutoMapper;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;

public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, MessageDto>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public CreateMessageCommandHandler(
        IMessageRepository messageRepository,
        IMapper mapper)
    {
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    public async Task<MessageDto> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        var messageEntity = _mapper.Map<Domain.Message>(request);
        await _messageRepository.CreateAsync(messageEntity);
        var messageDto = _mapper.Map<MessageDto>(messageEntity);
        return messageDto;
    }
}

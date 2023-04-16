using AutoMapper;
using LetsTalk.Server.API.Models;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Commands.CreateMessageCommand;

public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, CreateMessageResponse>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMessageProcessor _messageProcessor;
    private readonly IMapper _mapper;

    public CreateMessageCommandHandler(
        IMessageRepository messageRepository,
        IMessageProcessor messageProcessor,
        IMapper mapper)
    {
        _messageRepository = messageRepository;
        _messageProcessor = messageProcessor;
        _mapper = mapper;
    }

    public async Task<CreateMessageResponse> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        var message = _mapper.Map<Domain.Message>(request);
        _messageProcessor.SetTextHtml(message, out string? url);
        await _messageRepository.CreateAsync(message);
        return new CreateMessageResponse
        {
            Dto = _mapper.Map<MessageDto>(message),
            Url = url
        };
    }
}

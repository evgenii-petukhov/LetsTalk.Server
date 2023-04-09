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
    private readonly IMapper _mapper;
    private readonly IHtmlGenerator _messageProcessor;

    public CreateMessageCommandHandler(
        IMessageRepository messageRepository,
        IMapper mapper,
        IHtmlGenerator messageProcessor)
    {
        _messageRepository = messageRepository;
        _mapper = mapper;
        _messageProcessor = messageProcessor;
    }

    public async Task<CreateMessageResponse> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        var messageEntity = _mapper.Map<Domain.Message>(request);
        var htmlResult = _messageProcessor.GetHtml(messageEntity.Text!);
        messageEntity.TextHtml = htmlResult.Html;
        await _messageRepository.CreateAsync(messageEntity)
            .ConfigureAwait(false);
        return new CreateMessageResponse
        {
            Dto = _mapper.Map<MessageDto>(messageEntity),
            Url = htmlResult.Url
        };
    }
}

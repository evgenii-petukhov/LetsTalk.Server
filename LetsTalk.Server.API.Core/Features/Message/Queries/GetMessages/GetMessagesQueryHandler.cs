using AutoMapper;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Middleware.Exceptions;
using LetsTalk.Server.Dto.Models;
using MediatR;

namespace LetsTalk.Server.API.Core.Features.Message.Queries.GetMessages;

public class GetMessagesQueryHandler(
    IMapper mapper,
    IMessageService messageService,
    IChatService chatService) : IRequestHandler<GetMessagesQuery, List<MessageDto>>
{
    private readonly IMapper _mapper = mapper;
    private readonly IMessageService _messageService = messageService;
    private readonly IChatService _chatService = chatService;

    public async Task<List<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var validator = new GetMessagesQueryValidator(_chatService);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new NotFoundException($"Chat with id '{request.ChatId}' was not found.");
        }

        var messages = await _messageService.GetPagedAsync(
            request.ChatId,
            request.PageIndex,
            request.MessagesPerPage,
            cancellationToken);

        return [.. messages
            .Select(message =>
            {
                var messageDto = _mapper.Map<MessageDto>(message);
                messageDto.IsMine = message.SenderId == request.SenderId;
                return messageDto;
            })];
    }
}

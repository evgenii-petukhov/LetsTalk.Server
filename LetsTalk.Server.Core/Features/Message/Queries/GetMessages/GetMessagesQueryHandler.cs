using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.Abstractions;
using LetsTalk.Server.Persistence.DatabaseContext;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Queries.GetMessages;

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, List<MessageDto>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMessageProcessor _messageProcessor;
    private readonly IMapper _mapper;
    private readonly LetsTalkDbContext _context;

    public GetMessagesQueryHandler(
        IMessageRepository messageRepository,
        IMessageProcessor messageProcessor,
        IMapper mapper,
        LetsTalkDbContext context)
    {
        _messageRepository = messageRepository;
        _messageProcessor = messageProcessor;
        _mapper = mapper;
        _context = context;
    }

    public async Task<List<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        await _messageRepository.MarkAllAsReadAsync(request.SenderId, request.RecipientId, cancellationToken);

        var messages = await _messageRepository.GetAsync(request.SenderId, request.RecipientId, request.PageIndex, request.MessagesPerPage, cancellationToken);

        var messagesToProcess = messages
            .Where(message => message.TextHtml == null)
            .ToList();

        Parallel.ForEach(messagesToProcess, message => _messageProcessor.SetTextHtml(message, out _));

        foreach (var message in messagesToProcess)
        {
            await _messageRepository.SetTextHtmlAsync(message.Id, message.TextHtml!, cancellationToken: cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);

        return _mapper.Map<List<MessageDto>>(messages)
            .ConvertAll(messageDto =>
            {
                messageDto.IsMine = messageDto.SenderId == request.SenderId;
                return messageDto;
            });
    }
}

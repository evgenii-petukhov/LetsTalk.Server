using AutoMapper;
using LetsTalk.Server.Core.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.Repository.Abstractions;
using MediatR;

namespace LetsTalk.Server.Core.Features.Message.Queries.GetMessages;

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, List<MessageDto>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMessageProcessor _messageProcessor;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetMessagesQueryHandler(
        IMessageRepository messageRepository,
        IMessageProcessor messageProcessor,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _messageRepository = messageRepository;
        _messageProcessor = messageProcessor;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageRepository.GetPagedAsTrackingAsync(request.SenderId, request.RecipientId, request.PageIndex, request.MessagesPerPage, cancellationToken);

        messages
            .Where(message => !message.IsRead)
            .ToList()
            .ForEach(message => message.MarkAsRead());

        var messagesToProcess = messages
            .Where(message => message.TextHtml == null && !message.ImageId.HasValue)
            .ToList();

        Parallel.ForEach(messagesToProcess, message => _messageProcessor.SetTextHtml(message, out _));

        await _unitOfWork.SaveAsync(cancellationToken);

        return _mapper.Map<List<MessageDto>>(messages)
            .ConvertAll(messageDto =>
            {
                messageDto.IsMine = messageDto.SenderId == request.SenderId;
                return messageDto;
            });
    }
}

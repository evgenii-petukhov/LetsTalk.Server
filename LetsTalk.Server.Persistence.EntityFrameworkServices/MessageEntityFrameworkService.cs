using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.EntityFrameworkServices;

public class MessageEntityFrameworkService: IMessageAgnosticService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MessageEntityFrameworkService(
        IMessageRepository messageRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _messageRepository = messageRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<MessageAgnosticModel> CreateMessageAsync(
        int senderId,
        int recipientId,
        string? text,
        string? textHtml,
        int? imageId,
        CancellationToken cancellationToken)
    {
        var message = new Domain.Message(senderId, recipientId, text, textHtml, imageId);
        await _messageRepository.CreateAsync(message, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return _mapper.Map<MessageAgnosticModel>(message);
    }

    public async Task<List<MessageAgnosticModel>> GetPagedAsync(
        int senderId,
        int recipientId,
        int pageIndex,
        int messagesPerPage,
        CancellationToken cancellationToken = default)
    {
        var messages = await _messageRepository.GetPagedAsync(
            senderId,
            recipientId,
            pageIndex,
            messagesPerPage,
            cancellationToken);

        return _mapper.Map<List<MessageAgnosticModel>>(messages);
    }
}

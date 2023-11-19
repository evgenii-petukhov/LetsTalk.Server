using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.EntityFramework.Services;

public class MessageEntityFrameworkService : IMessageAgnosticService
{
    private readonly IMessageRepository _messageRepository;
    private readonly ILinkPreviewRepository _linkPreviewRepository;
    private readonly IEntityFactory _entityFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MessageEntityFrameworkService(
        IMessageRepository messageRepository,
        ILinkPreviewRepository linkPreviewRepository,
        IEntityFactory entityFactory,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _messageRepository = messageRepository;
        _linkPreviewRepository = linkPreviewRepository;
        _entityFactory = entityFactory;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<MessageServiceModel> CreateMessageAsync(
        string senderId,
        string recipientId,
        string text,
        string textHtml,
        string imageId,
        CancellationToken cancellationToken)
    {
        var message = new Domain.Message(
            int.Parse(senderId),
            int.Parse(recipientId),
            text, textHtml,
            string.IsNullOrWhiteSpace(imageId) ? null : int.Parse(imageId));
        await _messageRepository.CreateAsync(message, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return _mapper.Map<MessageServiceModel>(message);
    }

    public async Task<List<MessageServiceModel>> GetPagedAsync(
        string senderId,
        string recipientId,
        int pageIndex,
        int messagesPerPage,
        CancellationToken cancellationToken = default)
    {
        var messages = await _messageRepository.GetPagedAsync(
            int.Parse(senderId),
            int.Parse(recipientId),
            pageIndex,
            messagesPerPage,
            cancellationToken);

        return _mapper.Map<List<MessageServiceModel>>(messages);
    }

    public async Task<MessageServiceModel> SetLinkPreviewAsync(string messageId, string linkPreviewId, CancellationToken cancellationToken = default)
    {
        var linkPreview = await _linkPreviewRepository.GetByIdAsync(int.Parse(linkPreviewId), cancellationToken);
        var message = await _messageRepository.GetByIdAsTrackingAsync(int.Parse(messageId), cancellationToken);
        message.SetLinkPreview(linkPreview);
        await _unitOfWork.SaveAsync(cancellationToken);

        return _mapper.Map<MessageServiceModel>(message);
    }

    public async Task<MessageServiceModel> SetLinkPreviewAsync(
        string messageId,
        string url,
        string title,
        string imageUrl,
        CancellationToken cancellationToken = default)
    {
        var linkPreview = _entityFactory.CreateLinkPreview(url, title, imageUrl);
        var message = await _messageRepository.GetByIdAsTrackingAsync(int.Parse(messageId), cancellationToken);
        message.SetLinkPreview(linkPreview);
        await _unitOfWork.SaveAsync(cancellationToken);

        return _mapper.Map<MessageServiceModel>(message);
    }

    public async Task MarkAsRead(
        string messageId,
        string accountId,
        bool updatePreviousMessages,
        CancellationToken cancellationToken)
    {
        if (updatePreviousMessages)
        {
            await MarkAllAsRead(int.Parse(accountId), int.Parse(messageId), cancellationToken);
        }
        else
        {
            MarkAsRead(int.Parse(messageId));
        }
        await _unitOfWork.SaveAsync(cancellationToken);
    }

    private void MarkAsRead(int messageId)
    {
        var message = _entityFactory.CreateMessage(messageId);
        message.MarkAsRead();
    }

    private async Task MarkAllAsRead(int recipientId, int messageId, CancellationToken cancellationToken)
    {
        var message = await _messageRepository.GetByIdAsync(messageId, cancellationToken);

        if (message == null || message.SenderId == recipientId || message.IsRead)
        {
            return;
        }

        await _messageRepository.MarkAllAsRead(message.SenderId, recipientId, messageId);
    }
}

using AutoMapper;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using LetsTalk.Server.Persistence.Enums;

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
        CancellationToken cancellationToken)
    {
        var message = new Message(
            int.Parse(senderId),
            int.Parse(recipientId),
            text,
            textHtml);
        await _messageRepository.CreateAsync(message, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return _mapper.Map<MessageServiceModel>(message);
    }

    public async Task<MessageServiceModel> CreateMessageAsync(
        string senderId,
        string recipientId,
        string text,
        string textHtml,
        string imageId,
        int width,
        int height,
        ImageFormats imageFormat,
        CancellationToken cancellationToken)
    {
        var image = _entityFactory.CreateImage(imageId, imageFormat, width, height);

        var message = new Message(
            int.Parse(senderId),
            int.Parse(recipientId),
            text,
            textHtml,
            image);
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

    public async Task MarkAsReadAsync(
        string messageId,
        string accountId,
        bool updatePreviousMessages,
        CancellationToken cancellationToken)
    {
        if (updatePreviousMessages)
        {
            await MarkAllAsReadAsync(int.Parse(accountId), int.Parse(messageId), cancellationToken);
        }
        else
        {
            MarkAsRead(int.Parse(messageId));
        }
        await _unitOfWork.SaveAsync(cancellationToken);
    }

    public async Task<MessageServiceModel> SaveImagePreviewAsync(
        string messageId,
        string filename,
        ImageFormats imageFormat,
        int width,
        int height,
        CancellationToken cancellationToken = default)
    {
        var image = _entityFactory.CreateImage(filename, imageFormat, width, height);
        var message = await _messageRepository.GetByIdAsTrackingAsync(int.Parse(messageId), cancellationToken);
        message.SetImagePreview(image);

        await _unitOfWork.SaveAsync(cancellationToken);

        return _mapper.Map<MessageServiceModel>(message);
    }

    private void MarkAsRead(int messageId)
    {
        var message = _entityFactory.CreateMessage(messageId);
        message.MarkAsRead();
    }

    private async Task MarkAllAsReadAsync(int recipientId, int messageId, CancellationToken cancellationToken)
    {
        var message = await _messageRepository.GetByIdAsync(messageId, cancellationToken);

        if (message == null || message.SenderId == recipientId || message.IsRead)
        {
            return;
        }

        await _messageRepository.MarkAllAsReadAsync(message.SenderId, recipientId, messageId);
    }
}

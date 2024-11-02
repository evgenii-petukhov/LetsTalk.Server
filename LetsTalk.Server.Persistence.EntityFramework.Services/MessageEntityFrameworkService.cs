using AutoMapper;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.Server.Persistence.EntityFramework.Services;

public class MessageEntityFrameworkService(
    IMessageRepository messageRepository,
    IChatMessageStatusRepository chatMessageStatusRepository,
    ILinkPreviewRepository linkPreviewRepository,
    IEntityFactory entityFactory,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IMessageAgnosticService
{
    private readonly IMessageRepository _messageRepository = messageRepository;
    private readonly IChatMessageStatusRepository _chatMessageStatusRepository = chatMessageStatusRepository;
    private readonly ILinkPreviewRepository _linkPreviewRepository = linkPreviewRepository;
    private readonly IEntityFactory _entityFactory = entityFactory;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<MessageServiceModel> CreateMessageAsync(
        string senderId,
        string chatId,
        string text,
        string textHtml,
        string linkPreviewId,
        CancellationToken cancellationToken)
    {
        var message = new Message(
            int.Parse(senderId),
            int.Parse(chatId),
            text,
            textHtml,
            int.TryParse(linkPreviewId, out int linkPreviewIdInt) ? linkPreviewIdInt : null);
        await _messageRepository.CreateAsync(message, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);
        return _mapper.Map<MessageServiceModel>(await _messageRepository.GetByIdAsync(message.Id, cancellationToken));
    }

    public async Task<MessageServiceModel> CreateMessageAsync(
        string senderId,
        string chatId,
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
            int.Parse(chatId),
            text,
            textHtml,
            image: image);
        await _messageRepository.CreateAsync(message, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return _mapper.Map<MessageServiceModel>(message);
    }

    public async Task<List<MessageServiceModel>> GetPagedAsync(
        string chatId,
        int pageIndex,
        int messagesPerPage,
        CancellationToken cancellationToken = default)
    {
        var messages = await _messageRepository.GetPagedAsync(
            int.Parse(chatId),
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

    public Task MarkAsReadAsync(
        string chatId,
        string accountId,
        string messageId,
        CancellationToken cancellationToken)
    {
        return MarkAsReadAsync(int.Parse(chatId), int.Parse(accountId), int.Parse(messageId), cancellationToken);
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

    private async Task MarkAsReadAsync(int chatId, int accountId, int messageId, CancellationToken cancellationToken = default)
    {
        var chatMessageStatus = _entityFactory.CreateChatMessageStatus(chatId, accountId, messageId);
        chatMessageStatus.MarkAsRead();
        await _chatMessageStatusRepository.CreateAsync(chatMessageStatus, cancellationToken);
        try
        {
            await _unitOfWork.SaveAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            chatMessageStatus = _entityFactory.CreateChatMessageStatus(chatId, accountId, messageId, true);
            chatMessageStatus.MarkAsRead();
            await _unitOfWork.SaveAsync(cancellationToken);
        }
    }
}

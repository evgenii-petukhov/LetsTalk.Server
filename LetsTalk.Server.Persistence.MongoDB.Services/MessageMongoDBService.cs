using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.MongoDB.Services;

public class MessageMongoDBService(
    IMessageRepository messageRepository,
    IMapper mapper) : IMessageAgnosticService
{
    private readonly IMessageRepository _messageRepository = messageRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<MessageServiceModel> CreateMessageAsync(
        string senderId,
        string recipientId,
        string text,
        string textHtml,
        CancellationToken cancellationToken)
    {
        var message = await _messageRepository.CreateAsync(senderId, recipientId, text, textHtml, cancellationToken);

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
        var message = await _messageRepository.CreateAsync(
            senderId,
            recipientId,
            text,
            textHtml,
            imageId,
            width,
            height,
            imageFormat,
            cancellationToken);

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
            senderId,
            recipientId,
            pageIndex,
            messagesPerPage,
            cancellationToken);

        return _mapper.Map<List<MessageServiceModel>>(messages);
    }

    public async Task<MessageServiceModel> SetLinkPreviewAsync(string messageId, string linkPreviewId, CancellationToken cancellationToken = default)
    {
        var message = await _messageRepository.SetLinkPreviewAsync(messageId, linkPreviewId, cancellationToken);

        return _mapper.Map<MessageServiceModel>(message);
    }

    public async Task<MessageServiceModel> SetLinkPreviewAsync(
        string messageId,
        string url,
        string title,
        string imageUrl,
        CancellationToken cancellationToken = default)
    {
        var message = await _messageRepository.SetLinkPreviewAsync(messageId, url, title, imageUrl, cancellationToken);

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
            await MarkAllAsReadAsync(accountId, messageId, cancellationToken);
        }
        else
        {
            await _messageRepository.MarkAsReadAsync(messageId, cancellationToken);
        }
    }

    public async Task<MessageServiceModel> SaveImagePreviewAsync(
        string messageId,
        string filename,
        ImageFormats imageFormat,
        int width,
        int height,
        CancellationToken cancellationToken = default)
    {
        var message = await _messageRepository.SetImagePreviewAsync(
            messageId,
            filename,
            imageFormat,
            width,
            height,
            cancellationToken);

        return _mapper.Map<MessageServiceModel>(message);
    }

    private async Task MarkAllAsReadAsync(string recipientId, string messageId, CancellationToken cancellationToken)
    {
        var message = await _messageRepository.GetByIdAsync(messageId, cancellationToken);

        if (message == null || message.SenderId == recipientId || message.IsRead)
        {
            return;
        }

        await _messageRepository.MarkAllAsReadAsync(message.SenderId!, recipientId, message.DateCreatedUnix!.Value, cancellationToken);
    }
}

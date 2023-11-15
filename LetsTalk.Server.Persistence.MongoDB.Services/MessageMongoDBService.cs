using AutoMapper;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;

namespace LetsTalk.Server.Persistence.MongoDB.Services;

public class MessageMongoDBService : IMessageAgnosticService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public MessageMongoDBService(
        IMessageRepository messageRepository,
        IMapper mapper)
    {
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    public async Task<MessageServiceModel> CreateMessageAsync(
        string senderId,
        string recipientId,
        string? text,
        string? textHtml,
        int? imageId,
        CancellationToken cancellationToken)
    {
        var message = await _messageRepository.CreateAsync(senderId, recipientId, text, textHtml, imageId, cancellationToken);

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

    public async Task<MessageServiceModel> SetLinkPreviewAsync(string messageId, int linkPreviewId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<MessageServiceModel> SetLinkPreviewAsync(
        string messageId,
        string url,
        string title,
        string imageUrl,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task MarkAsRead(
        string messageId,
        string recipientId,
        bool updatePreviousMessages,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

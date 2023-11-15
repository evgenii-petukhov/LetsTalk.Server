using LetsTalk.Server.Persistence.MongoDB.Models;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;
using LetsTalk.Server.Persistence.Utility;
using MongoDB.Driver;

namespace LetsTalk.Server.Persistence.MongoDB.Repository;

public class MessageRepository : IMessageRepository
{
    private readonly IMongoCollection<Message> _messageCollection;

    public MessageRepository(IMongoClient mongoClient)
    {
        var mongoDatabase = mongoClient.GetDatabase("LetsTalk");

        _messageCollection = mongoDatabase.GetCollection<Message>(nameof(Message));
    }

    public Task<List<Message>> GetPagedAsync(string senderId, string recipientId, int pageIndex, int messagesPerPage, CancellationToken cancellationToken = default)
    {
        var filterDefinition = Builders<Message>.Filter
            .Where(message => (message.SenderId == senderId && message.RecipientId == recipientId) || (message.SenderId == recipientId && message.RecipientId == senderId));

        return _messageCollection
            .Find(filterDefinition)
            .SortByDescending(mesage => mesage.DateCreatedUnix)
            .Skip(messagesPerPage * pageIndex)
            .Limit(messagesPerPage)
            .SortBy(mesage => mesage.DateCreatedUnix)
            .ToListAsync(cancellationToken);
    }

    public async Task<Message> CreateAsync(
        string senderId,
        string recipientId,
        string? text,
        string? textHtml,
        int? imageId,
        CancellationToken cancellationToken = default)
    {
        var message = new Message
        {
            SenderId =  senderId,
            RecipientId = recipientId,
            Text = text,
            TextHtml = textHtml,
            ImageId = imageId,
            DateCreatedUnix = DateHelper.GetUnixTimestamp()
        };

        await _messageCollection.InsertOneAsync(message, cancellationToken: cancellationToken);

        return message;
    }
}

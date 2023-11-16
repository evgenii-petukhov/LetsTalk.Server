using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Persistence.MongoDB.Models;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;
using LetsTalk.Server.Persistence.Utility;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LetsTalk.Server.Persistence.MongoDB.Repository;

public class MessageRepository : IMessageRepository
{
    private readonly IMongoCollection<Message> _messageCollection;

    public MessageRepository(
        IMongoClient mongoClient,
        IOptions<MongoDBSettings> mongoDBSettings)
    {
        var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);

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

    public Task<Message> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _messageCollection
            .Find(Builders<Message>.Filter.Eq(x => x.Id, id))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task MarkAsRead(string messageId, CancellationToken cancellationToken = default)
    {
        return _messageCollection.UpdateOneAsync(
            Builders<Message>.Filter.Eq(x => x.Id, messageId),
            Builders<Message>.Update.Set(x => x.IsRead, true),
            cancellationToken: cancellationToken);
    }

    public Task MarkAllAsRead(string senderId, string recipientId, long dateCreatedUnix, CancellationToken cancellationToken = default)
    {
        return _messageCollection.UpdateManyAsync(
            Builders<Message>.Filter.Where(message => message.DateCreatedUnix <= dateCreatedUnix && message.SenderId == senderId && message.RecipientId == recipientId && !message.IsRead),
            Builders<Message>.Update.Set(x => x.IsRead, true),
            cancellationToken: cancellationToken);
    }
}

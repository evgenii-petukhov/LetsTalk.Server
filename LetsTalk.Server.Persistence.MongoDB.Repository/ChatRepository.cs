using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.MongoDB.Models;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LetsTalk.Server.Persistence.MongoDB.Repository;

public class ChatRepository : IChatRepository
{
    private readonly IMongoCollection<Chat> _chatCollection;
    private readonly IMongoCollection<Account> _accountCollection;
    private readonly IMongoCollection<Message> _messageCollection;
    private readonly IMongoCollection<ChatMessageStatus> _chatMessageStatusCollection;

    public ChatRepository(
        IMongoClient mongoClient,
        IOptions<MongoDBSettings> mongoDBSettings)
    {
        var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
        _chatCollection = mongoDatabase.GetCollection<Chat>(nameof(Chat));
        _accountCollection = mongoDatabase.GetCollection<Account>(nameof(Account));
        _messageCollection = mongoDatabase.GetCollection<Message>(nameof(Message));
        _chatMessageStatusCollection = mongoDatabase.GetCollection<ChatMessageStatus>(nameof(ChatMessageStatus));
    }

    public async Task<List<ChatServiceModel>> GetChatsAsync(string accountId, CancellationToken cancellationToken = default)
    {
        var chats = await _chatCollection
            .Find(Builders<Chat>.Filter.Where(x => x.AccountIds!.Contains(accountId)))
            .ToListAsync(cancellationToken);

        var accountIds = chats.SelectMany(x => x.AccountIds!)
            .Where(x => x != accountId)
            .Distinct()
            .ToList();

        var accounts = await _accountCollection
            .Find(Builders<Account>.Filter.In(x => x.Id, accountIds))
            .ToListAsync(cancellationToken);

        var messages = await _messageCollection
            .Find(Builders<Message>.Filter.In(x => x.ChatId, chats.Select(c => c.Id)))
            .ToListAsync(cancellationToken);

        var messageStatuses = await _chatMessageStatusCollection
            .Find(Builders<ChatMessageStatus>.Filter.In(x => x.ChatId, chats.Select(c => c.Id)))
            .Project(x => new ChatMessageStatus
            {
                ChatId = x.ChatId,
                AccountId = x.AccountId,
                MessageId = x.MessageId,
                DateReadUnix = x.DateReadUnix
            })
            .ToListAsync(cancellationToken);

        var chatMetrics = messages.GroupBy(m => m.ChatId).Select(g => new
        {
            ChatId = g.Key,
            LastMessageDate = g.Max(m => m.DateCreatedUnix),
            LastMessageId = g.Max(m => m.Id),
            UnreadCount = g.Count(m => m.DateCreatedUnix > (messageStatuses.FirstOrDefault(s => s.ChatId == g.Key)?.DateReadUnix ?? 0) && m.SenderId != accountId)
        }).ToList();

        return chats.Select(chat =>
        {
            var metrics = chatMetrics.FirstOrDefault(m => m.ChatId == chat.Id);
            var otherAccount = accounts.FirstOrDefault(a => chat.IsIndividual && chat.AccountIds!.Contains(a.Id));

            return new ChatServiceModel
            {
                Id = chat.Id,
                ChatName = chat.IsIndividual ? $"{otherAccount?.FirstName} {otherAccount?.LastName}" : chat.Name,
                PhotoUrl = chat.IsIndividual ? otherAccount?.PhotoUrl : null,
                AccountTypeId = chat.IsIndividual ? otherAccount?.AccountTypeId : null,
                ImageId = chat.IsIndividual ? otherAccount?.Image?.Id : chat.Image?.Id,
                LastMessageDate = metrics?.LastMessageDate,
                LastMessageId = metrics?.LastMessageId,
                UnreadCount = metrics?.UnreadCount ?? 0,
                IsIndividual = chat.IsIndividual,
                AccountIds = chat.AccountIds!.Where(x => x != accountId).ToArray()
            };
        }).ToList();
    }


    public async Task<string[]> GetChatMemberAccountIdsAsync(string chatId, CancellationToken cancellationToken = default)
    {
        var chat = await _chatCollection
            .Find(Builders<Chat>.Filter.Eq(x => x.Id, chatId))
            .FirstOrDefaultAsync(cancellationToken);

        return chat.AccountIds!;
    }

    public Task<bool> IsChatIdValidAsync(string id, CancellationToken cancellationToken = default)
    {
        return _chatCollection
            .Find(Builders<Chat>.Filter.Eq(x => x.Id, id))
            .AnyAsync(cancellationToken);
    }

    public Task<Chat> GetIndividualChatByAccountIdsAsync(string[] accountIds, CancellationToken cancellationToken = default)
    {
        return _chatCollection
            .Aggregate()
            .Match(c => c.IsIndividual && c.AccountIds!.All(accountId => accountIds.Contains(accountId)))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Chat> CreateIndividualChatAsync(string[] accountIds, CancellationToken cancellationToken = default)
    {
        var chat = new Chat
        {
            IsIndividual = true,
            AccountIds = accountIds
        };

        await _chatCollection.InsertOneAsync(chat, cancellationToken: cancellationToken);

        return chat;
    }
}

using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Persistence.MongoDB.Models;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions.Models;
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

    public Task<List<Chat>> GetChatsByAccountIdAsync(string accountId, CancellationToken cancellationToken = default)
    {
        return _chatCollection
            .Find(Builders<Chat>.Filter.Where(x => x.AccountIds!.Contains(accountId)))
            .ToListAsync(cancellationToken);
    }

    public Task<List<Account>> GetAccountsByChatsAsync(IEnumerable<Chat> chats, string accountId, CancellationToken cancellationToken = default)
    {
        var accountIds = chats
            .SelectMany(x => x.AccountIds!)
            .Where(x => !string.Equals(x, accountId, StringComparison.Ordinal))
            .Distinct()
            .ToHashSet();

        return _accountCollection
            .Find(Builders<Account>.Filter.In(x => x.Id, accountIds))
            .ToListAsync(cancellationToken);
    }

    public Dictionary<string, ChatMetric> GetChatMetrics(string accountId, CancellationToken cancellationToken = default)
    {
        return _chatCollection
            .AsQueryable()
            .Where(x => x.AccountIds!.Contains(accountId))
            .GroupJoin(_messageCollection.AsQueryable(), x => x.Id, x => x.ChatId, (x, y) => new
            {
                ChatId = x.Id,
                Messages = y
            })
            .SelectMany(x => x.Messages.DefaultIfEmpty(), (x, y) => new
            {
                x.ChatId,
                Message = y
            })
            .GroupJoin(
                _chatMessageStatusCollection.AsQueryable(),
                x => x.Message!.Id,
                x => x.MessageId,
                (x, statuses) => new
                {
                    x.ChatId,
                    x.Message,
                    Statuses = statuses
                })
            .SelectMany(x => x.Statuses.Where(s => s.AccountId == accountId).DefaultIfEmpty(), (x, status) => new
            {
                x.ChatId,
                x.Message,
                status!.DateReadUnix
            })
            .GroupBy(x => x.ChatId)
            .Select(g => new
            {
                ChatId = g.Key,
                LastMessageDate = g.Max(x => x.Message!.DateCreatedUnix),
                LastReadMessageDate = g.First(x => x.DateReadUnix == g.Max(m => m.DateReadUnix)).Message!.DateCreatedUnix,
                Messages = g.Select(x => x.Message).ToList()
            })
            .Select(x => new ChatMetric
            {
                ChatId = x.ChatId,
                LastMessageDate = x.LastMessageDate,
                LastMessageId = x.Messages.First(m => m!.DateCreatedUnix == x.LastMessageDate)!.Id,
                UnreadCount = x.Messages.Count(m => m!.DateCreatedUnix > x.LastReadMessageDate && m!.SenderId != accountId)
            })
            .ToDictionary(x => x.ChatId!, StringComparer.Ordinal);
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

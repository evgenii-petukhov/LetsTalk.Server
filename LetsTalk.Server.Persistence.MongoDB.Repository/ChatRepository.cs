using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
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
        IOptions<DatabaseSettings> mongoDBSettings)
    {
        var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.MongoDatabaseName);

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

        var accountIds = chats
            .SelectMany(x => x.AccountIds!)
            .Where(x => !string.Equals(x, accountId, StringComparison.Ordinal))
            .Distinct()
            .ToList();

        var accounts = await _accountCollection
            .Find(Builders<Account>.Filter.Where(x => accountIds.Contains(x.Id!)))
            .ToListAsync(cancellationToken);

        var accountsByChat = chats
            .Select(chat => new
            {
                Chat = chat,
                Accounts = accounts.Where(x => chat.AccountIds!.Contains(x.Id)).ToList()
            })
            .ToDictionary(x => x.Chat, x => new
            {
                x.Accounts,
                FirstAccount = x.Accounts.FirstOrDefault()
            });

        var metrics = _chatCollection
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
                (x, y) => new
                {
                    x.ChatId,
                    x.Message,
                    Statuses = y
                })
            .SelectMany(x => x.Statuses.DefaultIfEmpty(), (x, y) => new
            {
                x.ChatId,
                x.Message,
                y!.DateReadUnix
            })
            .GroupBy(x => x.ChatId)
            .Select(g => new
            {
                ChatId = g.Key,
                LastMessageDate = g.Max(x => x.Message!.DateCreatedUnix),
                LastMessageId = g.Max(x => x.Message!.Id),
                LastReadMessageDate = g.Max(x => x.DateReadUnix)
            })
            .GroupJoin(_messageCollection.AsQueryable(), x => x.ChatId, x => x.ChatId, (x, y) => new
            {
                x.ChatId,
                x.LastMessageId,
                x.LastReadMessageDate,
                x.LastMessageDate,
                Messages = y
            })
            .SelectMany(x => x.Messages.DefaultIfEmpty(), (x, y) => new
            {
                x.ChatId,
                x.LastMessageId,
                x.LastReadMessageDate,
                x.LastMessageDate,
                Message = y
            })
            .GroupBy(x => new
            {
                x.ChatId,
                x.LastMessageId,
                x.LastReadMessageDate,
                x.LastMessageDate,
            })
            .Select(g => new
            {
                g.Key.ChatId,
                g.Key.LastMessageId,
                g.Key.LastMessageDate,
                UnreadCount = g.Count(x => x.Message!.DateCreatedUnix > x.LastReadMessageDate && x.Message.SenderId != accountId)
            })
            .ToList();

        return chats
            .Join(metrics, x => x.Id, x => x.ChatId, (x, y) => new
            {
                Chat = x,
                x.AccountIds,
                Metrics = y
            })
            .Select(g => new ChatServiceModel
            {
                Id = g.Chat.Id,
                ChatName = GetChatName(g.Chat, accountsByChat.GetValueOrDefault(g.Chat)?.Accounts),
                PhotoUrl = g.Chat.IsIndividual ? accountsByChat.GetValueOrDefault(g.Chat)?.FirstAccount?.PhotoUrl : null,
                AccountTypeId = g.Chat.IsIndividual ? accountsByChat.GetValueOrDefault(g.Chat)?.FirstAccount?.AccountTypeId : null,
                ImageId = g.Chat.IsIndividual ? accountsByChat.GetValueOrDefault(g.Chat)?.FirstAccount?.Image?.Id : g.Chat.ImageId,
                LastMessageDate = g.Metrics.LastMessageDate,
                LastMessageId = g.Metrics.LastMessageId,
                UnreadCount = g.Metrics.UnreadCount,
                IsIndividual = g.Chat.IsIndividual,
                AccountId = g.Chat.IsIndividual ? accountsByChat.GetValueOrDefault(g.Chat)?.FirstAccount?.Id : null
            })
            .ToList();
    }

    public async Task<string[]> GetChatMemberAccountIdsAsync(string chatId, CancellationToken cancellationToken = default)
    {
        var chat = await _chatCollection
            .Find(Builders<Chat>.Filter.Eq(x => x.Id, chatId))
            .FirstOrDefaultAsync(cancellationToken);

        return chat.AccountIds!;
    }

    private static string? GetChatName(Chat chat, List<Account>? accounts)
    {
        if ((accounts == null || accounts.Count == 0) && (chat.IsIndividual || string.IsNullOrEmpty(chat.Name)))
        {
            return null;
        }

        if (chat.IsIndividual)
        {
            return accounts?.Count > 0
                ? $"{accounts[0].FirstName} {accounts[0].LastName}"
                : null;
        }
        else
        {
            return string.IsNullOrEmpty(chat.Name)
                ? string.Join(',', accounts!.Select(a => $"{a!.FirstName} {a.LastName}"))
                : chat.Name;
        }
    }
}

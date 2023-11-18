using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.MongoDB.Models;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LetsTalk.Server.Persistence.MongoDB.Repository;

public class AccountRepository : IAccountRepository
{
    private readonly IMongoCollection<Account> _accountCollection;
    private readonly IMongoCollection<Message> _messageCollection;

    public AccountRepository(
        IMongoClient mongoClient,
        IOptions<DatabaseSettings> mongoDBSettings)
    {
        var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.MongoDatabaseName);

        _accountCollection = mongoDatabase.GetCollection<Account>(nameof(Account));
        _messageCollection = mongoDatabase.GetCollection<Message>(nameof(Message));
    }

    public Task<Account> GetByExternalIdAsync(
        string externalId,
        AccountTypes accountType,
        CancellationToken cancellationToken = default)
    {
        return _accountCollection
            .Find(Builders<Account>.Filter.Where(x => x.ExternalId == externalId && x.AccountTypeId == (int)accountType))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Account> CreateAccountAsync(
        string externalId,
        AccountTypes accountType,
        string firstName,
        string lastName,
        string email,
        string photoUrl,
        CancellationToken cancellationToken)
    {
        var account = new Account
        {
            AccountTypeId = (int)accountType,
            ExternalId = externalId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhotoUrl = photoUrl
        };

        await _accountCollection.InsertOneAsync(account, cancellationToken: cancellationToken);

        return account;
    }

    public Task<Account> SetupProfileAsync(
        string externalId,
        AccountTypes accountType,
        string firstName,
        string lastName,
        string email,
        string photoUrl,
        bool updateAvatar,
        CancellationToken cancellationToken)
    {
        var filterdefinition = Builders<Account>.Filter
            .Where(x => x.ExternalId == externalId && x.AccountTypeId == (int)accountType);

        var updateDefinition = Builders<Account>.Update
            .Set(x => x.FirstName, firstName)
            .Set(x => x.LastName, lastName)
            .Set(x => x.Email, email);

        updateDefinition = updateAvatar
            ? updateDefinition
            : updateDefinition.Set(x => x.PhotoUrl, photoUrl);

        return _accountCollection
            .FindOneAndUpdateAsync(filterdefinition, updateDefinition, new FindOneAndUpdateOptions<Account, Account>
            {
                ReturnDocument = ReturnDocument.After
            }, cancellationToken: cancellationToken);
    }

    public Task<Account> UpdateProfileAsync(
        string id,
        string firstName,
        string lastName,
        string email,
        string imageId,
        CancellationToken cancellationToken = default)
    {
        var filterdefinition = Builders<Account>.Filter
            .Eq(x => x.Id, id);

        var updateDefinition = Builders<Account>.Update
            .Set(x => x.FirstName, firstName)
            .Set(x => x.LastName, lastName)
            .Set(x => x.Email, email);

        updateDefinition = imageId != null
            ? updateDefinition
            : updateDefinition.Set(x => x.ImageId, imageId);

        return _accountCollection
            .FindOneAndUpdateAsync(filterdefinition, updateDefinition, new FindOneAndUpdateOptions<Account, Account>
            {
                ReturnDocument = ReturnDocument.After
            }, cancellationToken: cancellationToken);
    }

    public async Task<List<Contact>> GetContactsAsync(string id, CancellationToken cancellationToken = default)
    {
        var accounts = await _accountCollection
            .Find(Builders<Account>.Filter.Where(x => x.Id != id))
            .ToListAsync(cancellationToken);

        var lastMessageByConversation = await _messageCollection
            .Aggregate()
            .Match(x => x.SenderId == id || x.RecipientId == id)
            .Group(x => new
            {
                x.RecipientId,
                x.SenderId
            }, g => new
            {
                AccountIds = g.Key,
                LastMessageDate = g.Max(x => x.DateCreatedUnix),
                LastMessageId = g.Max(x => x.Id),
                UnreadCount = g.Key.RecipientId == id ? g.Count(x => !x.IsRead) : 0
            })
            .ToListAsync(cancellationToken);

        var lastMessageBySender = lastMessageByConversation
            .Select(g => new
            {
                AccountId = g.AccountIds.RecipientId == id ? g.AccountIds.SenderId : g.AccountIds.RecipientId,
                g.LastMessageDate,
                g.LastMessageId,
                g.UnreadCount
            })
            .GroupBy(g => g.AccountId)
            .Select(g => new
            {
                AccountId = g.Key,
                LastMessageDate = g.Max(x => x.LastMessageDate),
                LastMessageId = g.Max(x => x.LastMessageId),
                UnreadCount = g.Sum(x => x.UnreadCount)
            });

        return accounts
            .Where(account => account.Id != id)
            .GroupJoin(lastMessageBySender, x => x.Id, x => x.AccountId, (x, y) => new
            {
                Account = x,
                Metrics = y
            })
            .SelectMany(
                x => x.Metrics.DefaultIfEmpty(),
                (x, y) => new
                {
                    x.Account,
                    Metrics = y
                })
            .Select(x => new Contact
            {
                Id = x.Account.Id,
                FirstName = x.Account.FirstName,
                LastName = x.Account.LastName,
                PhotoUrl = x.Account.PhotoUrl,
                AccountTypeId = x.Account.AccountTypeId,
                LastMessageDate = x.Metrics?.LastMessageDate,
                LastMessageId = x.Metrics?.LastMessageId,
                UnreadCount = x.Metrics?.UnreadCount,
                ImageId = x.Account.ImageId
            })
            .ToList();
    }

    public Task<Account> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _accountCollection
            .Find(Builders<Account>.Filter.Eq(x => x.Id, id))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> IsAccountIdValidAsync(string id, CancellationToken cancellationToken = default)
    {
        return _accountCollection
            .Find(Builders<Account>.Filter.Eq(x => x.Id, id))
            .AnyAsync(cancellationToken);
    }
}

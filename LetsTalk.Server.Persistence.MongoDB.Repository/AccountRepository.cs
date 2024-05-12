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

    public Task SetupProfileAsync(
        string externalId,
        AccountTypes accountType,
        string firstName,
        string lastName,
        string email,
        string photoUrl,
        bool updateAvatar,
        CancellationToken cancellationToken)
    {
        var filter = Builders<Account>.Filter
            .Where(x => x.ExternalId == externalId && x.AccountTypeId == (int)accountType);

        var updateDefinition = Builders<Account>.Update
            .Set(x => x.FirstName, firstName)
            .Set(x => x.LastName, lastName)
            .Set(x => x.Email, email);

        updateDefinition = updateAvatar
            ? updateDefinition
            : updateDefinition.Set(x => x.PhotoUrl, photoUrl);

        return _accountCollection
            .UpdateOneAsync(filter, updateDefinition, cancellationToken: cancellationToken);
    }

    public Task<Account> UpdateProfileAsync(
        string id,
        string firstName,
        string lastName,
        string email,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<Account>.Filter
            .Eq(x => x.Id, id);

        var updateDefinition = Builders<Account>.Update
            .Set(x => x.FirstName, firstName)
            .Set(x => x.LastName, lastName)
            .Set(x => x.Email, email);

        return _accountCollection
            .FindOneAndUpdateAsync(filter, updateDefinition, new FindOneAndUpdateOptions<Account, Account>
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
        int width,
        int height,
        ImageFormats imageFormat,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<Account>.Filter
            .Eq(x => x.Id, id);

        var updateDefinition = Builders<Account>.Update
            .Set(x => x.FirstName, firstName)
            .Set(x => x.LastName, lastName)
            .Set(x => x.Email, email);

        if (!string.IsNullOrEmpty(imageId))
        {
            updateDefinition = updateDefinition.Set(x => x.Image, new Image
            {
                Id = imageId,
                Width = width,
                Height = height,
                ImageFormatId = (int)imageFormat
            });
        }

        return _accountCollection
            .FindOneAndUpdateAsync(filter, updateDefinition, new FindOneAndUpdateOptions<Account, Account>
            {
                ReturnDocument = ReturnDocument.After
            }, cancellationToken: cancellationToken);
    }

    public Task<Account> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _accountCollection
            .Find(Builders<Account>.Filter.Eq(x => x.Id, id))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<Account>> GetAccountsAsync(CancellationToken cancellationToken = default)
    {
        return _accountCollection
            .Find(Builders<Account>.Filter.Empty)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> IsAccountIdValidAsync(string id, CancellationToken cancellationToken = default)
    {
        return _accountCollection
            .Find(Builders<Account>.Filter.Eq(x => x.Id, id))
            .AnyAsync(cancellationToken);
    }

    public Task<Account> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _accountCollection
            .Find(Builders<Account>.Filter.Where(x => x.AccountTypeId == (int)AccountTypes.Email && x.Email == email.ToLower()))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Account> CreateAccountAsync(AccountTypes accountType, string email, CancellationToken cancellationToken)
    {
        var account = new Account
        {
            AccountTypeId = (int)accountType,
            Email = email
        };

        await _accountCollection.InsertOneAsync(account, cancellationToken: cancellationToken);

        return account;
    }
}

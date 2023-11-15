using LetsTalk.Server.Persistence.MongoDB.Models;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;
using MongoDB.Driver;

namespace LetsTalk.Server.Persistence.MongoDB.Repository;

public class AccountRepository : IAccountRepository
{
    private readonly IMongoCollection<Account> _accountCollection;

    public AccountRepository(IMongoClient mongoClient)
    {
        var mongoDatabase = mongoClient.GetDatabase("LetsTalk");

        _accountCollection = mongoDatabase.GetCollection<Account>(nameof(Account));
    }

    public Task<Account> GetByExternalIdAsync(
        string externalId,
        int accountTypeId,
        CancellationToken cancellationToken)
    {
        return _accountCollection
            .Find(Builders<Account>.Filter.Where(x => x.ExternalId == externalId && x.AccountTypeId == accountTypeId))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Account> CreateAccountAsync(
        string externalId,
        int accountTypeId,
        string firstName,
        string lastName,
        string email,
        string photoUrl,
        CancellationToken cancellationToken)
    {
        var account = new Account
        {
            AccountTypeId = accountTypeId,
            ExternalId = externalId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhotoUrl = photoUrl
        };

        await _accountCollection.InsertOneAsync(account, cancellationToken: cancellationToken);

        return account;
    }

    public Task<Account> SetupProfile(
        string externalId,
        int accountTypeId,
        string firstName,
        string lastName,
        string email,
        string photoUrl,
        CancellationToken cancellationToken)
    {
        return _accountCollection
            .FindOneAndUpdateAsync(Builders<Account>.Filter.Where(x => x.ExternalId == externalId && x.AccountTypeId == accountTypeId), Builders<Account>.Update
                .Set(x => x.FirstName, firstName)
                .Set(x => x.LastName, lastName)
                .Set(x => x.Email, email)
                .Set(x => x.PhotoUrl, photoUrl), cancellationToken: cancellationToken);
    }

    public Task<List<Contact>> GetContactsAsync(string id, CancellationToken cancellationToken = default)
    {
        return _accountCollection
            .Find(Builders<Account>.Filter.Where(x => x.Id != id))
            .Project(x => new Contact
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                PhotoUrl = x.PhotoUrl,
                AccountTypeId = x.AccountTypeId,
                ImageId = x.ImageId
            })
            .ToListAsync(cancellationToken);
    }

    public Task<Account> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _accountCollection
            .Find(Builders<Account>.Filter.Where(x => x.Id == id))
            .FirstOrDefaultAsync(cancellationToken);
    }
}

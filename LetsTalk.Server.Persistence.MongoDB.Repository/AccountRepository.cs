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

    public Task<Account> GetByExternalIdAsync(string externalId, int accountTypeId, CancellationToken cancellationToken)
    {
        return _accountCollection
            .Find(x => x.ExternalId == externalId && x.AccountTypeId == accountTypeId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async Task<Account> CreateAccountAsync(string externalId, int accountTypeId, string firstName, string lastName, string email, string photoUrl)
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

        await _accountCollection.InsertOneAsync(account);

        return account;
    }
}

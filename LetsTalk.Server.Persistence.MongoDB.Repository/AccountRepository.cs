﻿using LetsTalk.Server.Configuration.Models;
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
        IOptions<MongoDBSettings> mongoDBSettings)
    {
        var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);

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

    public Task<Account> UpdateProfileAsync(
        string id,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<Account>.Filter
            .Eq(x => x.Id, id);

        var updateDefinition = Builders<Account>.Update
            .Set(x => x.FirstName, firstName)
            .Set(x => x.LastName, lastName);

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
        string imageId,
        int width,
        int height,
        ImageFormats imageFormat,
        FileStorageTypes fileStorageType,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<Account>.Filter
            .Eq(x => x.Id, id);

        var updateDefinition = Builders<Account>.Update
            .Set(x => x.FirstName, firstName)
            .Set(x => x.LastName, lastName);

        if (!string.IsNullOrEmpty(imageId))
        {
            updateDefinition = updateDefinition.Set(x => x.Image, new Image
            {
                Id = imageId,
                Width = width,
                Height = height,
                ImageFormatId = (int)imageFormat,
                FileStorageTypeId = (int)fileStorageType
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

    public Task<Account> GetByEmailAsync(string email, AccountTypes accountType, CancellationToken cancellationToken = default)
    {
        return _accountCollection
            .Find(Builders<Account>.Filter.Where(x => x.AccountTypeId == (int)accountType && x.Email!.Equals(email, StringComparison.OrdinalIgnoreCase)))
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

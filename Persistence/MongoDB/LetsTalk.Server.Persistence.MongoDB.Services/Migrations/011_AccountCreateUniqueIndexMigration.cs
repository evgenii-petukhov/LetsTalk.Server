using LetsTalk.Server.Persistence.MongoDB.Models;
using MongoDB.Driver;
using SimpleMongoMigrations.Abstractions;
using SimpleMongoMigrations.Attributes;

namespace LetsTalk.Server.Persistence.MongoDB.Services.Migrations;

[Version("0.1.1")]
[Name("Account: Create a unique index (AccountTypeId + ExternalId + Email)")]
public class AccountCreateUniqueIndexMigration : IMigration
{
    public Task UpAsync(IMongoDatabase database, CancellationToken cancellationToken)
    {
        return database.GetCollection<Account>(nameof(Account)).Indexes.CreateOneAsync(new CreateIndexModel<Account>(
            Builders<Account>.IndexKeys
                .Ascending(x => x.AccountTypeId)
                .Ascending(x => x.ExternalId)
                .Ascending(x => x.Email),
            new CreateIndexOptions
            {
                Unique = true,
            }), cancellationToken: cancellationToken);
    }
}
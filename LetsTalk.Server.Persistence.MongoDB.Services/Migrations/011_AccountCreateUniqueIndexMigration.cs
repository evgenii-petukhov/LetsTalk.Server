using LetsTalk.Server.Persistence.MongoDB.Models;
using MongoDB.Driver;
using MongoDBMigrations;

namespace LetsTalk.Server.Persistence.MongoDB.Services.Migrations;

public class AccountCreateUniqueIndexMigration : IMigration
{
    public MongoDBMigrations.Version Version => new(0, 1, 1);

    public string Name => "Account: Create a unique index (AccountTypeId + ExternalId + Email)";

    public void Down(IMongoDatabase database)
    {
        throw new NotImplementedException();
    }

    public void Up(IMongoDatabase database)
    {
        database.GetCollection<Account>(nameof(Account)).Indexes.CreateOne(new CreateIndexModel<Account>(
            Builders<Account>.IndexKeys
                .Ascending(x => x.AccountTypeId)
                .Ascending(x => x.ExternalId)
                .Ascending(x => x.Email),
            new CreateIndexOptions
            {
                Unique = true,
            }));
    }
}

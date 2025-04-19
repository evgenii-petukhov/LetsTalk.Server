using LetsTalk.Server.Persistence.MongoDB.Models;
using MongoDB.Driver;
using SimpleMongoMigrations;

namespace LetsTalk.Server.Persistence.MongoDB.Services.Migrations;

[Order(2)]
public class AccountCreateUniqueIndexMigration : IMigration
{
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
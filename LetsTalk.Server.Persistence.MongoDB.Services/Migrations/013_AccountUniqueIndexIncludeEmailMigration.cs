using LetsTalk.Server.Persistence.MongoDB.Models;
using MongoDB.Driver;
using MongoDBMigrations;

namespace LetsTalk.Server.Persistence.MongoDB.Services.Migrations;

public class AccountUniqueIndexIncludeEmailMigration : IMigration
{
    public MongoDBMigrations.Version Version => new(0, 1, 3);

    public string Name => "Account: include the Email field to the unique index";

    public void Down(IMongoDatabase database)
    {
        throw new NotImplementedException();
    }

    public void Up(IMongoDatabase database)
    {
        database.GetCollection<Account>(nameof(Account)).Indexes.DropOne("AccountTypeId_1_ExternalId_1");

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

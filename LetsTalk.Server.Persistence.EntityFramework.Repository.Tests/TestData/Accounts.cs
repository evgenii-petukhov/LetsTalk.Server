using LetsTalk.Server.Persistence.EntityFramework.Repository.Tests.Models;

namespace LetsTalk.Server.Persistence.EntityFramework.Repository.Tests.TestData;

public static class Accounts
{
    public static AccountModel NeilJohnston { get; } = new AccountModel
    {
        Email = "neil.johnston@hotmail.com",
        FirstName = "Neil",
        LastName = "Johnston",
        ImageId = "212fa20f-4c00-4952-8815-e0679dc123ed",
        FileStorageType = Enums.FileStorageTypes.Local
    };

    public static AccountModel BobPettit { get; } = new AccountModel
    {
        Email = "bob.pettit@hotmail.com",
        FirstName = "Bob",
        LastName = "Pettit",
        ImageId = "e88615fb-6c73-4bbf-b81f-c9bec45a7df2",
        FileStorageType = Enums.FileStorageTypes.AmazonS3
    };

    public static AccountModel RickBarry { get; } = new AccountModel
    {
        Email = "rick.barry@hotmail.com",
        FirstName = "Rick",
        LastName = "Barry",
    };

    public static AccountModel GeorgeGervin { get; } = new AccountModel
    {
        Email = "george.gervin@hotmail.com",
        FirstName = "George",
        LastName = "Gervin",
    };
}

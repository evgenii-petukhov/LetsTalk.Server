using FluentAssertions;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.MongoDB.Models;
using LetsTalk.Server.Persistence.MongoDB.Repository;
using LetsTalk.Server.Persistence.MongoDB.Services;
using LetsTalk.Server.Persistence.MongoDB.Tests.Models;
using LetsTalk.Server.Persistence.MongoDB.Tests.TestData;
using LetsTalk.Server.Utility.Common;
using Microsoft.Extensions.Options;
using Mongo2Go;
using MongoDB.Driver;
using SimpleMongoMigrations;
using System.Reflection;

namespace LetsTalk.Server.Persistence.MongoDB.Tests;

[TestFixture]
public class ChatMongoDBServiceTests
{
    private MongoDbRunner _runner;

    private IMongoCollection<Chat> _chatCollection;
    private IMongoCollection<Account> _accountCollection;
    private IMongoCollection<Message> _messageCollection;
    private IMongoCollection<ChatMessageStatus> _chatMessageStatusCollection;

    private ChatMongoDBService _service;
    private ChatRepository _chatRepository;

    private Account NeilJohnston;
    private Account BobPettit;
    private Account RickBarry;
    private Account GeorgeGervin;

    [SetUp]
    public async Task SetUp()
    {
        _runner = MongoDbRunner.Start();
        var client = new MongoClient(_runner.ConnectionString);
        var database = client.GetDatabase("LetsTalk");
        _chatCollection = database.GetCollection<Chat>(nameof(Chat));
        _accountCollection = database.GetCollection<Account>(nameof(Account));
        _messageCollection = database.GetCollection<Message>(nameof(Message));
        _chatMessageStatusCollection = database.GetCollection<ChatMessageStatus>(nameof(ChatMessageStatus));

        NeilJohnston = CreateAcccount(Accounts.NeilJohnston);
        BobPettit = CreateAcccount(Accounts.BobPettit);
        RickBarry = CreateAcccount(Accounts.RickBarry);
        GeorgeGervin = CreateAcccount(Accounts.GeorgeGervin);

        _accountCollection.InsertMany([NeilJohnston, BobPettit, RickBarry, GeorgeGervin]);

        await MigrationEngineBuilder
            .Create()
            .WithDatabase("LetsTalk")
            .WithAssembly(typeof(ChatMongoDBService).Assembly)
            .WithClient(client)
            .Build()
            .RunAsync(default);

        _chatRepository = new ChatRepository(client, Options.Create(new MongoDBSettings
        {
            DatabaseName = "LetsTalk"
        }));
        _service = new ChatMongoDBService(_chatRepository);
    }

    [TearDown]
    public void TearDown()
    {
        _runner.Dispose();
    }

    [Test]
    public async Task GetChatsAsync_ShouldReturnCorrectChatNames()
    {
        // Arrange
        var neilWithBob = new Chat
        {
            AccountIds = [NeilJohnston.Id!, BobPettit.Id!],
            IsIndividual = true
        };
        var neilWithRick = new Chat
        {
            AccountIds = [NeilJohnston.Id!, RickBarry.Id!],
            IsIndividual = true
        };
        var bobWithRick = new Chat
        {
            AccountIds = [BobPettit.Id!, RickBarry.Id!],
            IsIndividual = true
        };
        _chatCollection.InsertMany([neilWithBob, neilWithRick, bobWithRick]);

        // Act (as Neil Johnston)
        var chats = await _service.GetChatsAsync(NeilJohnston.Id!);

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithBob.Id,
                ChatName = $"{BobPettit.FirstName} {BobPettit.LastName}",
                Image = new ImageServiceModel
                {
                    Id = BobPettit.Image!.Id,
                    FileStorageTypeId = (int)FileStorageTypes.AmazonS3
                },
                IsIndividual = true,
                UnreadCount = 0,
                AccountIds = [BobPettit.Id!],
                AccountTypeId = (int)AccountTypes.Email
            },
            new() {
                Id = neilWithRick.Id,
                ChatName = $"{RickBarry.FirstName} {RickBarry.LastName}",
                IsIndividual = true,
                UnreadCount = 0,
                AccountIds = [RickBarry.Id!],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (as Bob Pettit)
        chats = await _service.GetChatsAsync(BobPettit.Id!);

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithBob.Id,
                ChatName = $"{NeilJohnston.FirstName} {NeilJohnston.LastName}",
                Image = new ImageServiceModel
                {
                    Id = NeilJohnston.Image!.Id,
                    FileStorageTypeId = (int)FileStorageTypes.Local
                },
                IsIndividual = true,
                UnreadCount = 0,
                AccountIds = [NeilJohnston.Id!],
                AccountTypeId = (int)AccountTypes.Email
            },
            new() {
                Id = bobWithRick.Id,
                ChatName = $"{RickBarry.FirstName} {RickBarry.LastName}",
                IsIndividual = true,
                UnreadCount = 0,
                AccountIds = [RickBarry.Id!],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (as Rick Barry)
        chats = await _service.GetChatsAsync(RickBarry.Id!);

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithRick.Id,
                ChatName = $"{NeilJohnston.FirstName} {NeilJohnston.LastName}",
                Image = new ImageServiceModel
                {
                    Id = NeilJohnston.Image.Id,
                    FileStorageTypeId = (int)FileStorageTypes.Local
                },
                IsIndividual = true,
                UnreadCount = 0,
                AccountIds = [NeilJohnston.Id!],
                AccountTypeId = (int)AccountTypes.Email
            },
            new() {
                Id = bobWithRick.Id,
                ChatName = $"{BobPettit.FirstName} {BobPettit.LastName}",
                Image = new ImageServiceModel
                {
                    Id = BobPettit.Image.Id,
                    FileStorageTypeId = (int)FileStorageTypes.AmazonS3
                },
                IsIndividual = true,
                UnreadCount = 0,
                AccountIds = [BobPettit.Id!],
                AccountTypeId = (int)AccountTypes.Email
            },
        ]);

        // Act (as George Gervin)
        chats = await _service.GetChatsAsync(GeorgeGervin.Id!);

        // Assert
        chats.Should().BeEmpty();
    }

    [Test]
    public async Task GetChatsAsync_ShouldReturnCorrectUnreadMessageCountAndLastMessageId()
    {
        // Arrange
        var neilWithBob = new Chat
        {
            AccountIds = [NeilJohnston.Id!, BobPettit.Id!],
            IsIndividual = true
        };
        var neilWithRick = new Chat
        {
            AccountIds = [NeilJohnston.Id!, RickBarry.Id!],
            IsIndividual = true
        };
        _chatCollection.InsertMany([neilWithBob, neilWithRick]);

        var startDate = DateHelper.GetUnixTimestamp() - 60;

        var messages = new[]
        {
            new Message
            {
                SenderId = NeilJohnston.Id,
                ChatId = neilWithBob.Id,
                Text = "Hi Bob",
                TextHtml = "<p>Hi Bob</p>",
                DateCreatedUnix = startDate
            },// 0
            new Message
            {
                SenderId = BobPettit.Id,
                ChatId = neilWithBob.Id,
                Text = "Hi Neil",
                TextHtml = "<p>Hi Neil</p>",
                DateCreatedUnix = ++startDate
            },// 1
            new Message
            {
                SenderId = NeilJohnston.Id,
                ChatId = neilWithBob.Id,
                Text = "How is it going?",
                TextHtml = "<p>How is it going?</p>",
                DateCreatedUnix = ++startDate
            },// 2
            new Message
            {
                SenderId = BobPettit.Id,
                ChatId = neilWithBob.Id,
                Text = "Fine",
                TextHtml = "<p>Fine</p>",
                DateCreatedUnix = ++startDate
            },// 3
            new Message
            {
                SenderId = BobPettit.Id,
                ChatId = neilWithBob.Id,
                Text = "Thanks",
                TextHtml = "<p>Thanks</p>",
                DateCreatedUnix = ++startDate
            },// 4
            new Message
            {
                SenderId = NeilJohnston.Id,
                ChatId = neilWithRick.Id,
                Text = "Hi Rick",
                TextHtml = "<p>Hi Rick</p>",
                DateCreatedUnix = ++startDate
            },// 5
            new Message
            {
                SenderId = RickBarry.Id,
                ChatId = neilWithRick.Id,
                Text = "Hi Neil",
                TextHtml = "<p>Hi Neil</p>",
                DateCreatedUnix = ++startDate
            },// 6
            new Message
            {
                SenderId = NeilJohnston.Id,
                ChatId = neilWithRick.Id,
                Text = "What's up?",
                TextHtml = "<p>What's up?</p>",
                DateCreatedUnix = ++startDate
            },// 7
            new Message
            {
                SenderId = RickBarry.Id,
                ChatId = neilWithRick.Id,
                Text = "Great",
                TextHtml = "<p>Great</p>",
                DateCreatedUnix = ++startDate
            },// 8
            new Message
            {
                SenderId = RickBarry.Id,
                ChatId = neilWithRick.Id,
                Text = "Thanks",
                TextHtml = "<p>Thanks</p>",
                DateCreatedUnix = ++startDate
            },// 9
            new Message
            {
                SenderId = RickBarry.Id,
                ChatId = neilWithRick.Id,
                Text = "What's the weather like in your city?",
                TextHtml = "<p>What's the weather like in your city?</p>",
                DateCreatedUnix = ++startDate
            },// 10
            new Message
            {
                SenderId = NeilJohnston.Id,
                ChatId = neilWithRick.Id,
                Text = "It's sunny",
                TextHtml = "<p>It's sunny</p>",
                DateCreatedUnix = ++startDate
            }// 11
        };

        _messageCollection.InsertMany(messages);

        // Act (as Neil Johnston)
        var chats = await _service.GetChatsAsync(NeilJohnston.Id!);

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithRick.Id,
                ChatName = $"{RickBarry.FirstName} {RickBarry.LastName}",
                IsIndividual = true,
                UnreadCount = 4,
                LastMessageId = messages[11].Id,
                LastMessageDate = messages[11].DateCreatedUnix,
                AccountIds = [RickBarry.Id!],
                AccountTypeId = (int)AccountTypes.Email
            },
            new() {
                Id = neilWithBob.Id,
                ChatName = $"{BobPettit.FirstName} {BobPettit.LastName}",
                Image = new ImageServiceModel
                {
                    Id = BobPettit.Image!.Id,
                    FileStorageTypeId = (int)FileStorageTypes.AmazonS3
                },
                IsIndividual = true,
                UnreadCount = 3,
                LastMessageId = messages[4].Id,
                LastMessageDate = messages[4].DateCreatedUnix,
                AccountIds = [BobPettit.Id!],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (as Bob Pettit)
        chats = await _service.GetChatsAsync(BobPettit.Id!);

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithBob.Id!,
                ChatName = $"{NeilJohnston.FirstName} {NeilJohnston.LastName}",
                Image = new ImageServiceModel
                {
                    Id = NeilJohnston.Image!.Id,
                    FileStorageTypeId = (int)FileStorageTypes.Local
                },
                IsIndividual = true,
                UnreadCount = 2,
                LastMessageId = messages[4].Id,
                LastMessageDate = messages[4].DateCreatedUnix,
                AccountIds = [NeilJohnston.Id!],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (as Rick Barry)
        chats = await _service.GetChatsAsync(RickBarry.Id!);

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithRick.Id,
                ChatName = $"{NeilJohnston.FirstName} {NeilJohnston.LastName}",
                Image = new ImageServiceModel
                {
                    Id = NeilJohnston.Image.Id,
                    FileStorageTypeId = (int)FileStorageTypes.Local
                },
                IsIndividual = true,
                UnreadCount = 3,
                LastMessageId = messages[11].Id,
                LastMessageDate = messages[11].DateCreatedUnix,
                AccountIds = [NeilJohnston.Id!],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (as George Gervin)
        chats = await _service.GetChatsAsync(GeorgeGervin.Id!);

        // Assert
        chats.Should().BeEmpty();

        // Act (Neil Johnston reads all messages in a chat with Bob Pettit)
        _chatMessageStatusCollection.InsertOne(new ChatMessageStatus
        {
            ChatId = neilWithBob.Id,
            AccountId = NeilJohnston.Id,
            MessageId = messages[4].Id,
            DateReadUnix = ++startDate
        });

        // Act (as Neil Johnston)
        chats = await _service.GetChatsAsync(NeilJohnston.Id!);

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithRick.Id,
                ChatName = $"{RickBarry.FirstName} {RickBarry.LastName}",
                IsIndividual = true,
                UnreadCount = 4,
                LastMessageId = messages[11].Id,
                LastMessageDate = messages[11].DateCreatedUnix,
                AccountIds = [RickBarry.Id!],
                AccountTypeId = (int)AccountTypes.Email
            },
            new() {
                Id = neilWithBob.Id,
                ChatName = $"{BobPettit.FirstName} {BobPettit.LastName}",
                Image = new ImageServiceModel
                {
                    Id = BobPettit.Image.Id,
                    FileStorageTypeId = (int)FileStorageTypes.AmazonS3
                },
                IsIndividual = true,
                UnreadCount = 0,
                LastMessageId = messages[4].Id,
                LastMessageDate = messages[4].DateCreatedUnix,
                AccountIds = [BobPettit.Id!],
                AccountTypeId = (int)AccountTypes.Email,
            }
        ]);

        // Act (as Bob Pettit)
        chats = await _service.GetChatsAsync(BobPettit.Id!);

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithBob.Id,
                ChatName = $"{NeilJohnston.FirstName} {NeilJohnston.LastName}",
                Image = new ImageServiceModel
                {
                    Id = NeilJohnston.Image.Id,
                    FileStorageTypeId = (int)FileStorageTypes.Local
                },
                IsIndividual = true,
                UnreadCount = 2,
                LastMessageId = messages[4].Id,
                LastMessageDate = messages[4].DateCreatedUnix,
                AccountIds = [NeilJohnston.Id!],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (as Rick Barry)
        chats = await _service.GetChatsAsync(RickBarry.Id!);

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithRick.Id,
                ChatName = $"{NeilJohnston.FirstName} {NeilJohnston.LastName}",
                Image = new ImageServiceModel
                {
                    Id = NeilJohnston.Image.Id,
                    FileStorageTypeId = (int)FileStorageTypes.Local
                },
                IsIndividual = true,
                UnreadCount = 3,
                LastMessageId = messages[11].Id,
                LastMessageDate = messages[11].DateCreatedUnix,
                AccountIds = [NeilJohnston.Id!],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (Rick Barry reads all messages in a chat with Neil Johnston)
        _chatMessageStatusCollection.InsertOne(new ChatMessageStatus
        {
            ChatId = neilWithRick.Id,
            AccountId = RickBarry.Id,
            MessageId = messages[11].Id,
            DateReadUnix = ++startDate
        });

        // Act (as Rick Barry)
        chats = await _service.GetChatsAsync(RickBarry.Id!);

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithRick.Id,
                ChatName = $"{NeilJohnston.FirstName} {NeilJohnston.LastName}",
                Image = new ImageServiceModel
                {
                    Id = NeilJohnston.Image.Id,
                    FileStorageTypeId = (int)FileStorageTypes.Local
                },
                IsIndividual = true,
                UnreadCount = 0,
                LastMessageId = messages[11].Id,
                LastMessageDate = messages[11].DateCreatedUnix,
                AccountIds = [NeilJohnston.Id!],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (as Neil Johnston)
        chats = await _service.GetChatsAsync(NeilJohnston.Id!);

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithRick.Id,
                ChatName = $"{RickBarry.FirstName} {RickBarry.LastName}",
                IsIndividual = true,
                UnreadCount = 4,
                LastMessageId = messages[11].Id,
                LastMessageDate = messages[11].DateCreatedUnix,
                AccountIds = [RickBarry.Id!],
                AccountTypeId = (int)AccountTypes.Email
            },
            new() {
                Id = neilWithBob.Id,
                ChatName = $"{BobPettit.FirstName} {BobPettit.LastName}",
                Image = new ImageServiceModel
                {
                    Id = BobPettit.Image.Id,
                    FileStorageTypeId = (int)FileStorageTypes.AmazonS3
                },
                IsIndividual = true,
                UnreadCount = 0,
                LastMessageId = messages[4].Id,
                LastMessageDate = messages[4].DateCreatedUnix,
                AccountIds = [BobPettit.Id!],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (Neil Johnston reads all messages in a chat with Rick Barry)
        _chatMessageStatusCollection.InsertOne(new ChatMessageStatus
        {
            ChatId = neilWithRick.Id,
            AccountId = NeilJohnston.Id,
            MessageId = messages[10].Id,
            DateReadUnix = ++startDate
        });

        // Act (as Neil Johnston)
        chats = await _service.GetChatsAsync(NeilJohnston.Id!);

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithRick.Id,
                ChatName = $"{RickBarry.FirstName} {RickBarry.LastName}",
                IsIndividual = true,
                UnreadCount = 0,
                LastMessageId = messages[11].Id,
                LastMessageDate = messages[11].DateCreatedUnix,
                AccountIds = [RickBarry.Id!],
                AccountTypeId = (int)AccountTypes.Email
            },
            new() {
                Id = neilWithBob.Id,
                ChatName = $"{BobPettit.FirstName} {BobPettit.LastName}",
                Image = new ImageServiceModel
                {
                    Id = BobPettit.Image.Id,
                    FileStorageTypeId = (int)FileStorageTypes.AmazonS3
                },
                IsIndividual = true,
                UnreadCount = 0,
                LastMessageId = messages[4].Id,
                LastMessageDate = messages[4].DateCreatedUnix,
                AccountIds = [BobPettit.Id!],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (as Rick Barry)
        chats = await _service.GetChatsAsync(RickBarry.Id!);

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithRick.Id,
                ChatName = $"{NeilJohnston.FirstName} {NeilJohnston.LastName}",
                Image = new ImageServiceModel
                {
                    Id = NeilJohnston.Image.Id,
                    FileStorageTypeId = (int)FileStorageTypes.Local
                },
                IsIndividual = true,
                UnreadCount = 0,
                LastMessageId = messages[11].Id,
                LastMessageDate = messages[11].DateCreatedUnix,
                AccountIds = [NeilJohnston.Id!],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);
    }

    [Test]
    public async Task GetChatsAsync_ShouldReturnCorrectUnreadMessageCountAndLastMessageId_WithStatuses()
    {
        // Arrange
        var neilWithBob = new Chat
        {
            AccountIds = [NeilJohnston.Id!, BobPettit.Id!],
            IsIndividual = true
        };
        var neilWithRick = new Chat
        {
            AccountIds = [NeilJohnston.Id!, RickBarry.Id!],
            IsIndividual = true
        };
        _chatCollection.InsertMany([neilWithBob, neilWithRick]);

        var startDate = DateHelper.GetUnixTimestamp() - 60;

        var messages = new[]
        {
            new Message
            {
                SenderId = NeilJohnston.Id,
                ChatId = neilWithBob.Id,
                Text = "Hi Bob",
                TextHtml = "<p>Hi Bob</p>",
                DateCreatedUnix = startDate
            },// 0
            new Message
            {
                SenderId = BobPettit.Id,
                ChatId = neilWithBob.Id,
                Text = "Hi Neil",
                TextHtml = "<p>Hi Neil</p>",
                DateCreatedUnix = ++startDate
            },// 1
            new Message
            {
                SenderId = NeilJohnston.Id,
                ChatId = neilWithBob.Id,
                Text = "How is it going?",
                TextHtml = "<p>How is it going?</p>",
                DateCreatedUnix = ++startDate
            },// 2
            new Message
            {
                SenderId = BobPettit.Id,
                ChatId = neilWithBob.Id,
                Text = "Fine",
                TextHtml = "<p>Fine</p>",
                DateCreatedUnix = ++startDate
            },// 3
            new Message
            {
                SenderId = BobPettit.Id,
                ChatId = neilWithBob.Id,
                Text = "Thanks",
                TextHtml = "<p>Thanks</p>",
                DateCreatedUnix = ++startDate
            },// 4
            new Message
            {
                SenderId = NeilJohnston.Id,
                ChatId = neilWithRick.Id,
                Text = "Hi Rick",
                TextHtml = "<p>Hi Rick</p>",
                DateCreatedUnix = ++startDate
            },// 5
            new Message
            {
                SenderId = RickBarry.Id,
                ChatId = neilWithRick.Id,
                Text = "Hi Neil",
                TextHtml = "<p>Hi Neil</p>",
                DateCreatedUnix = ++startDate
            },// 6
            new Message
            {
                SenderId = NeilJohnston.Id,
                ChatId = neilWithRick.Id,
                Text = "What's up?",
                TextHtml = "<p>What's up?</p>",
                DateCreatedUnix = ++startDate
            },// 7
            new Message
            {
                SenderId = RickBarry.Id,
                ChatId = neilWithRick.Id,
                Text = "Great",
                TextHtml = "<p>Great</p>",
                DateCreatedUnix = ++startDate
            },// 8
            new Message
            {
                SenderId = RickBarry.Id,
                ChatId = neilWithRick.Id,
                Text = "Thanks",
                TextHtml = "<p>Thanks</p>",
                DateCreatedUnix = ++startDate
            },// 9
            new Message
            {
                SenderId = RickBarry.Id,
                ChatId = neilWithRick.Id,
                Text = "What's the weather like in your city?",
                TextHtml = "<p>What's the weather like in your city?</p>",
                DateCreatedUnix = ++startDate
            },// 10
            new Message
            {
                SenderId = NeilJohnston.Id,
                ChatId = neilWithRick.Id,
                Text = "It's sunny",
                TextHtml = "<p>It's sunny</p>",
                DateCreatedUnix = ++startDate
            }// 11
        };

        _messageCollection.InsertMany(messages);

        var statuses = new[]
        {
            new ChatMessageStatus
            {
                ChatId = neilWithBob.Id,
                AccountId = NeilJohnston.Id,
                MessageId = messages[1].Id,
                DateReadUnix = ++startDate
            },
            new ChatMessageStatus
            {
                ChatId = neilWithBob.Id,
                AccountId = NeilJohnston.Id,
                MessageId = messages[3].Id,
                DateReadUnix = ++startDate
            },
            new ChatMessageStatus
            {
                ChatId = neilWithBob.Id,
                AccountId = BobPettit.Id,
                MessageId = messages[0].Id,
                DateReadUnix = ++startDate
            },
            new ChatMessageStatus
            {
                ChatId = neilWithRick.Id,
                AccountId = NeilJohnston.Id,
                MessageId = messages[6].Id,
                DateReadUnix = ++startDate
            },
            new ChatMessageStatus
            {
                ChatId = neilWithRick.Id,
                AccountId = NeilJohnston.Id,
                MessageId = messages[8].Id,
                DateReadUnix = ++startDate
            },
            new ChatMessageStatus
            {
                ChatId = neilWithRick.Id,
                AccountId = NeilJohnston.Id,
                MessageId = messages[9].Id,
                DateReadUnix = ++startDate
            },
            new ChatMessageStatus
            {
                ChatId = neilWithRick.Id,
                AccountId = NeilJohnston.Id,
                MessageId = messages[10].Id,
                DateReadUnix = ++startDate
            },
        };
        _chatMessageStatusCollection.InsertMany(statuses);

        // Act (as Neil Johnston)
        var chats = await _service.GetChatsAsync(NeilJohnston.Id!);

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithRick.Id,
                ChatName = $"{RickBarry.FirstName} {RickBarry.LastName}",
                IsIndividual = true,
                UnreadCount = 0,
                LastMessageId = messages[11].Id,
                LastMessageDate = messages[11].DateCreatedUnix,
                AccountIds = [RickBarry.Id!],
                AccountTypeId = (int)AccountTypes.Email
            },
            new() {
                Id = neilWithBob.Id,
                ChatName = $"{BobPettit.FirstName} {BobPettit.LastName}",
                Image = new ImageServiceModel
                {
                    Id = BobPettit.Image!.Id,
                    FileStorageTypeId = (int)FileStorageTypes.AmazonS3
                },
                IsIndividual = true,
                UnreadCount = 1,
                LastMessageId = messages[4].Id,
                LastMessageDate = messages[4].DateCreatedUnix,
                AccountIds = [BobPettit.Id!],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (as Bob Pettit)
        chats = await _service.GetChatsAsync(BobPettit.Id!);

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithBob.Id,
                ChatName = $"{NeilJohnston.FirstName} {NeilJohnston.LastName}",
                Image = new ImageServiceModel
                {
                    Id = NeilJohnston.Image!.Id,
                    FileStorageTypeId = (int)FileStorageTypes.Local
                },
                IsIndividual = true,
                UnreadCount = 1,
                LastMessageId = messages[4].Id,
                LastMessageDate = messages[4].DateCreatedUnix,
                AccountIds = [NeilJohnston.Id!],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (as Rick Barry)
        chats = await _service.GetChatsAsync(RickBarry.Id!);

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithRick.Id,
                ChatName = $"{NeilJohnston.FirstName} {NeilJohnston.LastName}",
                Image = new ImageServiceModel
                {
                    Id = NeilJohnston.Image.Id,
                    FileStorageTypeId = (int)FileStorageTypes.Local
                },
                IsIndividual = true,
                UnreadCount = 3,
                LastMessageId = messages[11].Id,
                LastMessageDate = messages[11].DateCreatedUnix,
                AccountIds = [NeilJohnston.Id!],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (as George Gervin)
        chats = await _service.GetChatsAsync(GeorgeGervin.Id!);

        // Assert
        chats.Should().BeEmpty();
    }

    private static Account CreateAcccount(AccountModel accountModel)
    {
        return new Account
        {
            AccountTypeId = (int)AccountTypes.Email,
            Email = accountModel.Email!,
            FirstName = accountModel.FirstName!,
            LastName = accountModel.LastName!,
            Image = string.IsNullOrEmpty(accountModel.ImageId) ? null : new Image
            {
                Id = accountModel.ImageId,
                ImageFormatId = (int)ImageFormats.Webp,
                Height = 100,
                Width = 100,
                FileStorageTypeId = (int)accountModel.FileStorageType!
            }
        };
    }
}

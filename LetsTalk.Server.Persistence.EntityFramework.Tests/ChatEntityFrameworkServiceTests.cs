using AutoMapper;
using FluentAssertions;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.DatabaseContext;
using LetsTalk.Server.Persistence.EntityFramework.Repository;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using LetsTalk.Server.Persistence.EntityFramework.Services;
using LetsTalk.Server.Persistence.EntityFramework.Tests.MappingProfiles;
using LetsTalk.Server.Persistence.EntityFramework.Tests.Models;
using LetsTalk.Server.Persistence.EntityFramework.Tests.TestData;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LetsTalk.Server.Persistence.EntityFramework.Tests;

[TestFixture]
public class ChatEntityFrameworkServiceTests
{
    private LetsTalkDbContext _context;
    private ChatEntityFrameworkService _service;
    private ChatRepository _chatRepository;
    private IMapper _mapper;
    private Account NeilJohnston;
    private Account BobPettit;
    private Account RickBarry;
    private Account GeorgeGervin;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<LetsTalkDbContext>()
            .UseInMemoryDatabase("LetsTalk")
            .Options;

        _context = new LetsTalkDbContext(options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        _chatRepository = new ChatRepository(_context);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ImageProfile>();
        });

        _mapper = config.CreateMapper();

        _service = new ChatEntityFrameworkService(
            _chatRepository,
            Mock.Of<IChatMemberRepository>(),
            Mock.Of<IUnitOfWork>(),
            _mapper);

        NeilJohnston = CreateAcccount(Accounts.NeilJohnston);
        BobPettit = CreateAcccount(Accounts.BobPettit);
        RickBarry = CreateAcccount(Accounts.RickBarry);
        GeorgeGervin = CreateAcccount(Accounts.GeorgeGervin);
        _context.Accounts.AddRange(NeilJohnston, BobPettit, RickBarry, GeorgeGervin);
    }

    [TearDown]
    public void TearDown()
    {
        _chatRepository.Dispose();
        _context.Dispose();
    }

    [Test]
    public async Task GetChatsAsync_ShouldReturnCorrectChatNames()
    {
        // Arrange
        var neilWithBob = new Chat([NeilJohnston.Id, BobPettit.Id]);
        var neilWithRick = new Chat([NeilJohnston.Id, RickBarry.Id]);
        var bobWithRick = new Chat([BobPettit.Id, RickBarry.Id]);

        _context.Chats.AddRange(neilWithBob, neilWithRick, bobWithRick);
        await _context.SaveChangesAsync();

        // Act (as Neil Johnston)
        var chats = await _service.GetChatsAsync(NeilJohnston.Id.ToString());

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithBob.Id.ToString(),
                ChatName = $"{BobPettit.FirstName} {BobPettit.LastName}",
                Image = new ImageServiceModel
                {
                    Id = BobPettit.ImageId,
                    FileStorageTypeId = (int)FileStorageTypes.AmazonS3
                },
                IsIndividual = true,
                UnreadCount = 0,
                AccountIds = [BobPettit.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email
            },
            new() {
                Id = neilWithRick.Id.ToString(),
                ChatName = $"{RickBarry.FirstName} {RickBarry.LastName}",
                IsIndividual = true,
                UnreadCount = 0,
                AccountIds = [RickBarry.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (as Bob Pettit)
        chats = await _service.GetChatsAsync(BobPettit.Id.ToString());

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithBob.Id.ToString(),
                ChatName = $"{NeilJohnston.FirstName} {NeilJohnston.LastName}",
                Image = new ImageServiceModel
                {
                    Id = NeilJohnston.ImageId,
                    FileStorageTypeId = (int)FileStorageTypes.Local
                },
                IsIndividual = true,
                UnreadCount = 0,
                AccountIds = [NeilJohnston.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email
            },
            new() {
                Id = bobWithRick.Id.ToString(),
                ChatName = $"{RickBarry.FirstName} {RickBarry.LastName}",
                IsIndividual = true,
                UnreadCount = 0,
                AccountIds = [RickBarry.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (as Rick Barry)
        chats = await _service.GetChatsAsync(RickBarry.Id.ToString());

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithRick.Id.ToString(),
                ChatName = $"{NeilJohnston.FirstName} {NeilJohnston.LastName}",
                Image = new ImageServiceModel
                {
                    Id = NeilJohnston.ImageId,
                    FileStorageTypeId = (int)FileStorageTypes.Local
                },
                IsIndividual = true,
                UnreadCount = 0,
                AccountIds = [NeilJohnston.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email
            },
            new() {
                Id = bobWithRick.Id.ToString(),
                ChatName = $"{BobPettit.FirstName} {BobPettit.LastName}",
                Image = new ImageServiceModel
                {
                    Id = BobPettit.ImageId,
                    FileStorageTypeId = (int)FileStorageTypes.AmazonS3
                },
                IsIndividual = true,
                UnreadCount = 0,
                AccountIds = [BobPettit.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email
            },
        ]);

        // Act (as George Gervin)
        chats = await _service.GetChatsAsync(GeorgeGervin.Id.ToString());

        // Assert
        chats.Should().BeEmpty();
    }

    [Test]
    public async Task GetChatsAsync_ShouldReturnCorrectUnreadMessageCountAndLastMessageId()
    {
        // Arrange
        var neilWithBob = new Chat([NeilJohnston.Id, BobPettit.Id]);
        var neilWithRick = new Chat([NeilJohnston.Id, RickBarry.Id]);
        _context.Chats.AddRange(neilWithBob, neilWithRick);

        var messages = new[]
        {
            new Message(NeilJohnston.Id, neilWithBob.Id, "Hi Bob", "<p>Hi Bob</p>"),// 0
            new Message(BobPettit.Id, neilWithBob.Id, "Hi Neil", "<p>Hi Neil</p>"),// 1
            new Message(NeilJohnston.Id, neilWithBob.Id, "How is it going?", "<p>How is it going?</p>"),// 2
            new Message(BobPettit.Id, neilWithBob.Id, "Fine", "<p>Fine</p>"),// 3
            new Message(BobPettit.Id, neilWithBob.Id, "Thanks", "<p>Thanks</p>"),// 4
            new Message(NeilJohnston.Id, neilWithRick.Id, "Hi Rick", "<p>Hi Rick</p>"),// 5
            new Message(RickBarry.Id, neilWithRick.Id, "Hi Neil", "<p>Hi Neil</p>"),// 6
            new Message(NeilJohnston.Id, neilWithRick.Id, "What's up?", "<p>What's up?</p>"),// 7
            new Message(RickBarry.Id, neilWithRick.Id, "Great", "<p>Great</p>"),// 8
            new Message(RickBarry.Id, neilWithRick.Id, "Thanks", "<p>Thanks</p>"),// 9
            new Message(RickBarry.Id, neilWithRick.Id, "What's the weather like in your city?", "<p>What's the weather like in your city?</p>"),// 10
            new Message(NeilJohnston.Id, neilWithRick.Id, "It's sunny", "<p>It's sunny</p>")// 11
        };

        _context.Messages.AddRange(messages);
        await _context.SaveChangesAsync();

        // Act (as Neil Johnston)
        var chats = await _service.GetChatsAsync(NeilJohnston.Id.ToString());

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithRick.Id.ToString(),
                ChatName = $"{RickBarry.FirstName} {RickBarry.LastName}",
                IsIndividual = true,
                UnreadCount = 4,
                LastMessageId = messages[11].Id.ToString(),
                LastMessageDate = messages[11].DateCreatedUnix,
                AccountIds = [RickBarry.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email
            },
            new() {
                Id = neilWithBob.Id.ToString(),
                ChatName = $"{BobPettit.FirstName} {BobPettit.LastName}",
                Image = new ImageServiceModel
                {
                    Id = BobPettit.ImageId,
                    FileStorageTypeId = (int)FileStorageTypes.AmazonS3
                },
                IsIndividual = true,
                UnreadCount = 3,
                LastMessageId = messages[4].Id.ToString(),
                LastMessageDate = messages[4].DateCreatedUnix,
                AccountIds = [BobPettit.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (as Bob Pettit)
        chats = await _service.GetChatsAsync(BobPettit.Id.ToString());

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithBob.Id.ToString(),
                ChatName = $"{NeilJohnston.FirstName} {NeilJohnston.LastName}",
                Image = new ImageServiceModel
                {
                    Id = NeilJohnston.ImageId,
                    FileStorageTypeId = (int)FileStorageTypes.Local
                },
                IsIndividual = true,
                UnreadCount = 2,
                LastMessageId = messages[4].Id.ToString(),
                LastMessageDate = messages[4].DateCreatedUnix,
                AccountIds = [NeilJohnston.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (as Rick Barry)
        chats = await _service.GetChatsAsync(RickBarry.Id.ToString());

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithRick.Id.ToString(),
                ChatName = $"{NeilJohnston.FirstName} {NeilJohnston.LastName}",
                Image = new ImageServiceModel
                {
                    Id = NeilJohnston.ImageId,
                    FileStorageTypeId = (int)FileStorageTypes.Local
                },
                IsIndividual = true,
                UnreadCount = 3,
                LastMessageId = messages[11].Id.ToString(),
                LastMessageDate = messages[11].DateCreatedUnix,
                AccountIds = [NeilJohnston.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (as George Gervin)
        chats = await _service.GetChatsAsync(GeorgeGervin.Id.ToString());

        // Assert
        chats.Should().BeEmpty();

        // Act (Neil Johnston reads all messages in a chat with Bob Pettit)
        _context.ChatMessageStatuses.Add(new ChatMessageStatus(neilWithBob.Id, NeilJohnston.Id, messages[4].Id));
        await _context.SaveChangesAsync();

        // Act (as Neil Johnston)
        chats = await _service.GetChatsAsync(NeilJohnston.Id.ToString());

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithRick.Id.ToString(),
                ChatName = $"{RickBarry.FirstName} {RickBarry.LastName}",
                IsIndividual = true,
                UnreadCount = 4,
                LastMessageId = messages[11].Id.ToString(),
                LastMessageDate = messages[11].DateCreatedUnix,
                AccountIds = [RickBarry.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email
            },
            new() {
                Id = neilWithBob.Id.ToString(),
                ChatName = $"{BobPettit.FirstName} {BobPettit.LastName}",
                Image = new ImageServiceModel
                {
                    Id = BobPettit.ImageId,
                    FileStorageTypeId = (int)FileStorageTypes.AmazonS3
                },
                IsIndividual = true,
                UnreadCount = 0,
                LastMessageId = messages[4].Id.ToString(),
                LastMessageDate = messages[4].DateCreatedUnix,
                AccountIds = [BobPettit.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email,
            }
        ]);

        // Act (as Bob Pettit)
        chats = await _service.GetChatsAsync(BobPettit.Id.ToString());

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithBob.Id.ToString(),
                ChatName = $"{NeilJohnston.FirstName} {NeilJohnston.LastName}",
                Image = new ImageServiceModel
                {
                    Id = NeilJohnston.ImageId,
                    FileStorageTypeId = (int)FileStorageTypes.Local
                },
                IsIndividual = true,
                UnreadCount = 2,
                LastMessageId = messages[4].Id.ToString(),
                LastMessageDate = messages[4].DateCreatedUnix,
                AccountIds = [NeilJohnston.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (as Rick Barry)
        chats = await _service.GetChatsAsync(RickBarry.Id.ToString());

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithRick.Id.ToString(),
                ChatName = $"{NeilJohnston.FirstName} {NeilJohnston.LastName}",
                Image = new ImageServiceModel
                {
                    Id = NeilJohnston.ImageId,
                    FileStorageTypeId = (int)FileStorageTypes.Local
                },
                IsIndividual = true,
                UnreadCount = 3,
                LastMessageId = messages[11].Id.ToString(),
                LastMessageDate = messages[11].DateCreatedUnix,
                AccountIds = [NeilJohnston.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (Rick Barry reads all messages in a chat with Neil Johnston)
        _context.ChatMessageStatuses.Add(new ChatMessageStatus(neilWithRick.Id, RickBarry.Id, messages[11].Id));
        await _context.SaveChangesAsync();

        // Act (as Rick Barry)
        chats = await _service.GetChatsAsync(RickBarry.Id.ToString());

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithRick.Id.ToString(),
                ChatName = $"{NeilJohnston.FirstName} {NeilJohnston.LastName}",
                Image = new ImageServiceModel
                {
                    Id = NeilJohnston.ImageId,
                    FileStorageTypeId = (int)FileStorageTypes.Local
                },
                IsIndividual = true,
                UnreadCount = 0,
                LastMessageId = messages[11].Id.ToString(),
                LastMessageDate = messages[11].DateCreatedUnix,
                AccountIds = [NeilJohnston.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (as Neil Johnston)
        chats = await _service.GetChatsAsync(NeilJohnston.Id.ToString());

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithRick.Id.ToString(),
                ChatName = $"{RickBarry.FirstName} {RickBarry.LastName}",
                IsIndividual = true,
                UnreadCount = 4,
                LastMessageId = messages[11].Id.ToString(),
                LastMessageDate = messages[11].DateCreatedUnix,
                AccountIds = [RickBarry.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email
            },
            new() {
                Id = neilWithBob.Id.ToString(),
                ChatName = $"{BobPettit.FirstName} {BobPettit.LastName}",
                Image = new ImageServiceModel
                {
                    Id = BobPettit.ImageId,
                    FileStorageTypeId = (int)FileStorageTypes.AmazonS3
                },
                IsIndividual = true,
                UnreadCount = 0,
                LastMessageId = messages[4].Id.ToString(),
                LastMessageDate = messages[4].DateCreatedUnix,
                AccountIds = [BobPettit.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (Neil Johnston reads all messages in a chat with Rick Barry)
        _context.ChatMessageStatuses.Add(new ChatMessageStatus(neilWithRick.Id, NeilJohnston.Id, messages[10].Id));
        await _context.SaveChangesAsync();

        // Act (as Neil Johnston)
        chats = await _service.GetChatsAsync(NeilJohnston.Id.ToString());

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithRick.Id.ToString(),
                ChatName = $"{RickBarry.FirstName} {RickBarry.LastName}",
                IsIndividual = true,
                UnreadCount = 0,
                LastMessageId = messages[11].Id.ToString(),
                LastMessageDate = messages[11].DateCreatedUnix,
                AccountIds = [RickBarry.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email
            },
            new() {
                Id = neilWithBob.Id.ToString(),
                ChatName = $"{BobPettit.FirstName} {BobPettit.LastName}",
                Image = new ImageServiceModel
                {
                    Id = BobPettit.ImageId,
                    FileStorageTypeId = (int)FileStorageTypes.AmazonS3
                },
                IsIndividual = true,
                UnreadCount = 0,
                LastMessageId = messages[4].Id.ToString(),
                LastMessageDate = messages[4].DateCreatedUnix,
                AccountIds = [BobPettit.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (as Rick Barry)
        chats = await _service.GetChatsAsync(RickBarry.Id.ToString());

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithRick.Id.ToString(),
                ChatName = $"{NeilJohnston.FirstName} {NeilJohnston.LastName}",
                Image = new ImageServiceModel
                {
                    Id = NeilJohnston.ImageId,
                    FileStorageTypeId = (int)FileStorageTypes.Local
                },
                IsIndividual = true,
                UnreadCount = 0,
                LastMessageId = messages[11].Id.ToString(),
                LastMessageDate = messages[11].DateCreatedUnix,
                AccountIds = [NeilJohnston.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);
    }

    [Test]
    public async Task GetChatsAsync_ShouldReturnCorrectUnreadMessageCountAndLastMessageId_WithStatuses()
    {
        // Arrange
        var neilWithBob = new Chat([NeilJohnston.Id, BobPettit.Id]);
        var neilWithRick = new Chat([NeilJohnston.Id, RickBarry.Id]);
        _context.Chats.AddRange(neilWithBob, neilWithRick);

        var messages = new[]
        {
            new Message(NeilJohnston.Id, neilWithBob.Id, "Hi Bob", "<p>Hi Bob</p>"),// 0
            new Message(BobPettit.Id, neilWithBob.Id, "Hi Neil", "<p>Hi Neil</p>"),// 1
            new Message(NeilJohnston.Id, neilWithBob.Id, "How is it going?", "<p>How is it going?</p>"),// 2
            new Message(BobPettit.Id, neilWithBob.Id, "Fine", "<p>Fine</p>"),// 3
            new Message(BobPettit.Id, neilWithBob.Id, "Thanks", "<p>Thanks</p>"),// 4
            new Message(NeilJohnston.Id, neilWithRick.Id, "Hi Rick", "<p>Hi Rick</p>"),// 5
            new Message(RickBarry.Id, neilWithRick.Id, "Hi Neil", "<p>Hi Neil</p>"),// 6
            new Message(NeilJohnston.Id, neilWithRick.Id, "What's up?", "<p>What's up?</p>"),// 7
            new Message(RickBarry.Id, neilWithRick.Id, "Great", "<p>Great</p>"),// 8
            new Message(RickBarry.Id, neilWithRick.Id, "Thanks", "<p>Thanks</p>"),// 9
            new Message(RickBarry.Id, neilWithRick.Id, "What's the weather like in your city?", "<p>What's the weather like in your city?</p>"),// 10
            new Message(NeilJohnston.Id, neilWithRick.Id, "It's sunny", "<p>It's sunny</p>")// 11
        };

        _context.Messages.AddRange(messages);

        var statuses = new[]
        {
            new ChatMessageStatus(neilWithBob.Id, NeilJohnston.Id, messages[1].Id),
            new ChatMessageStatus(neilWithBob.Id, NeilJohnston.Id, messages[3].Id),
            new ChatMessageStatus(neilWithBob.Id, BobPettit.Id, messages[0].Id),
            new ChatMessageStatus(neilWithRick.Id, NeilJohnston.Id, messages[6].Id),
            new ChatMessageStatus(neilWithRick.Id, NeilJohnston.Id, messages[8].Id),
            new ChatMessageStatus(neilWithRick.Id, NeilJohnston.Id, messages[9].Id),
            new ChatMessageStatus(neilWithRick.Id, NeilJohnston.Id, messages[10].Id),
        };
        _context.ChatMessageStatuses.AddRange(statuses);

        await _context.SaveChangesAsync();

        // Act (as Neil Johnston)
        var chats = await _service.GetChatsAsync(NeilJohnston.Id.ToString());

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithRick.Id.ToString(),
                ChatName = $"{RickBarry.FirstName} {RickBarry.LastName}",
                IsIndividual = true,
                UnreadCount = 0,
                LastMessageId = messages[11].Id.ToString(),
                LastMessageDate = messages[11].DateCreatedUnix,
                AccountIds = [RickBarry.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email
            },
            new() {
                Id = neilWithBob.Id.ToString(),
                ChatName = $"{BobPettit.FirstName} {BobPettit.LastName}",
                Image = new ImageServiceModel
                {
                    Id = BobPettit.ImageId,
                    FileStorageTypeId = (int)FileStorageTypes.AmazonS3
                },
                IsIndividual = true,
                UnreadCount = 1,
                LastMessageId = messages[4].Id.ToString(),
                LastMessageDate = messages[4].DateCreatedUnix,
                AccountIds = [BobPettit.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (as Bob Pettit)
        chats = await _service.GetChatsAsync(BobPettit.Id.ToString());

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithBob.Id.ToString(),
                ChatName = $"{NeilJohnston.FirstName} {NeilJohnston.LastName}",
                Image = new ImageServiceModel
                {
                    Id = NeilJohnston.ImageId,
                    FileStorageTypeId = (int)FileStorageTypes.Local
                },
                IsIndividual = true,
                UnreadCount = 1,
                LastMessageId = messages[4].Id.ToString(),
                LastMessageDate = messages[4].DateCreatedUnix,
                AccountIds = [NeilJohnston.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (as Rick Barry)
        chats = await _service.GetChatsAsync(RickBarry.Id.ToString());

        // Assert
        chats.Should().BeEquivalentTo<ChatServiceModel>(
        [
            new() {
                Id = neilWithRick.Id.ToString(),
                ChatName = $"{NeilJohnston.FirstName} {NeilJohnston.LastName}",
                Image = new ImageServiceModel
                {
                    Id = NeilJohnston.ImageId,
                    FileStorageTypeId = (int)FileStorageTypes.Local
                },
                IsIndividual = true,
                UnreadCount = 3,
                LastMessageId = messages[11].Id.ToString(),
                LastMessageDate = messages[11].DateCreatedUnix,
                AccountIds = [NeilJohnston.Id.ToString()],
                AccountTypeId = (int)AccountTypes.Email
            }
        ]);

        // Act (as George Gervin)
        chats = await _service.GetChatsAsync(GeorgeGervin.Id.ToString());

        // Assert
        chats.Should().BeEmpty();
    }

    private static Account CreateAcccount(AccountModel accountModel)
    {
        var account = new Account((int)AccountTypes.Email, accountModel.Email!);
        var image = string.IsNullOrEmpty(accountModel.ImageId)
            ? null
            : new Image(accountModel.ImageId, (int)ImageFormats.Webp, 100, 100, (int)accountModel.FileStorageType!);

        account.UpdateProfile(accountModel.FirstName!, accountModel.LastName!, image);

        return account;
    }
}

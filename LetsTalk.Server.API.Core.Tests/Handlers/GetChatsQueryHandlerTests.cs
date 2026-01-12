using AutoMapper;
using FluentAssertions;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Features.Chat.Queries.GetChats;
using LetsTalk.Server.Dto.Models;
using Moq;

namespace LetsTalk.Server.API.Core.Tests.Handlers;

[TestFixture]
public class GetChatsQueryHandlerTests
{
    private Mock<IMapper> _mapperMock;
    private Mock<IChatService> _chatServiceMock;
    private GetChatsQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _mapperMock = new Mock<IMapper>();
        _chatServiceMock = new Mock<IChatService>();
        _handler = new GetChatsQueryHandler(_mapperMock.Object, _chatServiceMock.Object);
    }

    [Test]
    public async Task Handle_WhenChatsExist_ShouldReturnMappedChats()
    {
        // Arrange
        var accountId = "account-123";
        var query = new GetChatsQuery(accountId);
        var cancellationToken = CancellationToken.None;

        var serviceChatDtos = new List<ChatDto>
        {
            new() { Id = "chat-1", ChatName = "Chat 1", UnreadCount = 5 },
            new() { Id = "chat-2", ChatName = "Chat 2", UnreadCount = 0 }
        }.AsReadOnly();

        var mappedChatDtos = new List<ChatDto>
        {
            new() { Id = "chat-1", ChatName = "Chat 1", UnreadCount = 5 },
            new() { Id = "chat-2", ChatName = "Chat 2", UnreadCount = 0 }
        };

        _chatServiceMock
            .Setup(x => x.GetChatsAsync(accountId, cancellationToken))
            .ReturnsAsync(serviceChatDtos);

        _mapperMock
            .Setup(x => x.Map<List<ChatDto>>(serviceChatDtos))
            .Returns(mappedChatDtos);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(mappedChatDtos);
    }

    [Test]
    public async Task Handle_WhenNoChatsExist_ShouldReturnEmptyList()
    {
        // Arrange
        var accountId = "account-123";
        var query = new GetChatsQuery(accountId);
        var cancellationToken = CancellationToken.None;

        var serviceChatDtos = new List<ChatDto>().AsReadOnly();
        var mappedChatDtos = new List<ChatDto>();

        _chatServiceMock
            .Setup(x => x.GetChatsAsync(accountId, cancellationToken))
            .ReturnsAsync(serviceChatDtos);

        _mapperMock
            .Setup(x => x.Map<List<ChatDto>>(serviceChatDtos))
            .Returns(mappedChatDtos);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Test]
    public async Task Handle_WhenChatsHaveComplexData_ShouldPreserveAllProperties()
    {
        // Arrange
        var accountId = "account-123";
        var query = new GetChatsQuery(accountId);
        var cancellationToken = CancellationToken.None;

        var imageDto = new ImageDto { Id = "image-1", FileStorageTypeId = 1 };
        var serviceChatDtos = new List<ChatDto>
        {
            new()
            {
                Id = "chat-1",
                ChatName = "Complex Chat",
                UnreadCount = 10,
                AccountTypeId = 1,
                PhotoUrl = "https://example.com/photo.jpg",
                LastMessageDate = 1640995200000,
                LastMessageId = "msg-123",
                Image = imageDto,
                IsIndividual = true,
                AccountIds = new List<string> { "acc-1", "acc-2" }
            }
        }.AsReadOnly();

        var mappedChatDtos = new List<ChatDto>
        {
            new()
            {
                Id = "chat-1",
                ChatName = "Complex Chat",
                UnreadCount = 10,
                AccountTypeId = 1,
                PhotoUrl = "https://example.com/photo.jpg",
                LastMessageDate = 1640995200000,
                LastMessageId = "msg-123",
                Image = imageDto,
                IsIndividual = true,
                AccountIds = new List<string> { "acc-1", "acc-2" }
            }
        };

        _chatServiceMock
            .Setup(x => x.GetChatsAsync(accountId, cancellationToken))
            .ReturnsAsync(serviceChatDtos);

        _mapperMock
            .Setup(x => x.Map<List<ChatDto>>(serviceChatDtos))
            .Returns(mappedChatDtos);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        
        var chat = result.First();
        chat.Id.Should().Be("chat-1");
        chat.ChatName.Should().Be("Complex Chat");
        chat.UnreadCount.Should().Be(10);
        chat.AccountTypeId.Should().Be(1);
        chat.PhotoUrl.Should().Be("https://example.com/photo.jpg");
        chat.LastMessageDate.Should().Be(1640995200000);
        chat.LastMessageId.Should().Be("msg-123");
        chat.Image.Should().NotBeNull();
        chat.Image!.Id.Should().Be("image-1");
        chat.Image.FileStorageTypeId.Should().Be(1);
        chat.IsIndividual.Should().BeTrue();
        chat.AccountIds.Should().NotBeNull();
        chat.AccountIds.Should().HaveCount(2);
        chat.AccountIds.Should().Contain("acc-1");
        chat.AccountIds.Should().Contain("acc-2");
    }

    [Test]
    public async Task Handle_WhenCancellationTokenProvided_ShouldPassItToService()
    {
        // Arrange
        var accountId = "account-123";
        var query = new GetChatsQuery(accountId);
        var cancellationToken = new CancellationToken(false);

        var serviceChatDtos = new List<ChatDto>().AsReadOnly();
        var mappedChatDtos = new List<ChatDto>();

        _chatServiceMock
            .Setup(x => x.GetChatsAsync(accountId, cancellationToken))
            .ReturnsAsync(serviceChatDtos);

        _mapperMock
            .Setup(x => x.Map<List<ChatDto>>(serviceChatDtos))
            .Returns(mappedChatDtos);

        // Act
        await _handler.Handle(query, cancellationToken);

        // Assert
        _chatServiceMock.Verify(
            x => x.GetChatsAsync(accountId, cancellationToken),
            Times.Once);
    }

    [Test]
    public void Handle_WhenChatServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var accountId = "account-123";
        var query = new GetChatsQuery(accountId);
        var cancellationToken = CancellationToken.None;

        _chatServiceMock
            .Setup(x => x.GetChatsAsync(accountId, cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Chat service error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, cancellationToken));
    }

    [Test]
    public void Handle_WhenMapperThrowsException_ShouldPropagateException()
    {
        // Arrange
        var accountId = "account-123";
        var query = new GetChatsQuery(accountId);
        var cancellationToken = CancellationToken.None;

        var serviceChatDtos = new List<ChatDto>
        {
            new() { Id = "chat-1", ChatName = "Chat 1" }
        }.AsReadOnly();

        _chatServiceMock
            .Setup(x => x.GetChatsAsync(accountId, cancellationToken))
            .ReturnsAsync(serviceChatDtos);

        _mapperMock
            .Setup(x => x.Map<List<ChatDto>>(serviceChatDtos))
            .Throws(new InvalidOperationException("Mapper error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, cancellationToken));
    }

    [Test]
    public async Task Handle_WhenServiceReturnsNull_ShouldHandleGracefully()
    {
        // Arrange
        var accountId = "account-123";
        var query = new GetChatsQuery(accountId);
        var cancellationToken = CancellationToken.None;

        _chatServiceMock
            .Setup(x => x.GetChatsAsync(accountId, cancellationToken))
            .ReturnsAsync((IReadOnlyList<ChatDto>)null!);

        _mapperMock
            .Setup(x => x.Map<List<ChatDto>>(It.IsAny<IReadOnlyList<ChatDto>>()))
            .Returns(new List<ChatDto>());

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _mapperMock.Verify(
            x => x.Map<List<ChatDto>>(null),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenMapperReturnsNull_ShouldReturnNull()
    {
        // Arrange
        var accountId = "account-123";
        var query = new GetChatsQuery(accountId);
        var cancellationToken = CancellationToken.None;

        var serviceChatDtos = new List<ChatDto>().AsReadOnly();

        _chatServiceMock
            .Setup(x => x.GetChatsAsync(accountId, cancellationToken))
            .ReturnsAsync(serviceChatDtos);

        _mapperMock
            .Setup(x => x.Map<List<ChatDto>>(serviceChatDtos))
            .Returns((List<ChatDto>)null!);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task Handle_WhenAccountIdIsEmpty_ShouldStillCallService()
    {
        // Arrange
        var accountId = "";
        var query = new GetChatsQuery(accountId);
        var cancellationToken = CancellationToken.None;

        var serviceChatDtos = new List<ChatDto>().AsReadOnly();
        var mappedChatDtos = new List<ChatDto>();

        _chatServiceMock
            .Setup(x => x.GetChatsAsync(accountId, cancellationToken))
            .ReturnsAsync(serviceChatDtos);

        _mapperMock
            .Setup(x => x.Map<List<ChatDto>>(serviceChatDtos))
            .Returns(mappedChatDtos);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _chatServiceMock.Verify(
            x => x.GetChatsAsync(accountId, cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenAccountIdIsNull_ShouldStillCallService()
    {
        // Arrange
        var query = new GetChatsQuery(null!);
        var cancellationToken = CancellationToken.None;

        var serviceChatDtos = new List<ChatDto>().AsReadOnly();
        var mappedChatDtos = new List<ChatDto>();

        _chatServiceMock
            .Setup(x => x.GetChatsAsync(null!, cancellationToken))
            .ReturnsAsync(serviceChatDtos);

        _mapperMock
            .Setup(x => x.Map<List<ChatDto>>(serviceChatDtos))
            .Returns(mappedChatDtos);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _chatServiceMock.Verify(
            x => x.GetChatsAsync(null!, cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenMultipleChatsWithDifferentTypes_ShouldReturnAll()
    {
        // Arrange
        var accountId = "account-123";
        var query = new GetChatsQuery(accountId);
        var cancellationToken = CancellationToken.None;

        var serviceChatDtos = new List<ChatDto>
        {
            new() { Id = "chat-1", ChatName = "Individual Chat", IsIndividual = true, UnreadCount = 3 },
            new() { Id = "chat-2", ChatName = "Group Chat", IsIndividual = false, UnreadCount = 0 },
            new() { Id = "chat-3", ChatName = "Another Individual", IsIndividual = true, UnreadCount = 1 }
        }.AsReadOnly();

        var mappedChatDtos = new List<ChatDto>
        {
            new() { Id = "chat-1", ChatName = "Individual Chat", IsIndividual = true, UnreadCount = 3 },
            new() { Id = "chat-2", ChatName = "Group Chat", IsIndividual = false, UnreadCount = 0 },
            new() { Id = "chat-3", ChatName = "Another Individual", IsIndividual = true, UnreadCount = 1 }
        };

        _chatServiceMock
            .Setup(x => x.GetChatsAsync(accountId, cancellationToken))
            .ReturnsAsync(serviceChatDtos);

        _mapperMock
            .Setup(x => x.Map<List<ChatDto>>(serviceChatDtos))
            .Returns(mappedChatDtos);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(c => c.Id == "chat-1" && c.IsIndividual);
        result.Should().Contain(c => c.Id == "chat-2" && !c.IsIndividual);
        result.Should().Contain(c => c.Id == "chat-3" && c.IsIndividual);
    }

    [Test]
    public async Task Handle_WhenValidRequest_ShouldFollowCompleteWorkflow()
    {
        // Arrange
        var accountId = "account-123";
        var query = new GetChatsQuery(accountId);
        var cancellationToken = CancellationToken.None;

        var serviceChatDtos = new List<ChatDto>
        {
            new() { Id = "chat-1", ChatName = "Test Chat", UnreadCount = 2 }
        }.AsReadOnly();

        var mappedChatDtos = new List<ChatDto>
        {
            new() { Id = "chat-1", ChatName = "Test Chat", UnreadCount = 2 }
        };

        _chatServiceMock
            .Setup(x => x.GetChatsAsync(accountId, cancellationToken))
            .ReturnsAsync(serviceChatDtos);

        _mapperMock
            .Setup(x => x.Map<List<ChatDto>>(serviceChatDtos))
            .Returns(mappedChatDtos);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Id.Should().Be("chat-1");

        // Verify the complete workflow
        _chatServiceMock.Verify(
            x => x.GetChatsAsync(accountId, cancellationToken),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<List<ChatDto>>(serviceChatDtos),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenServiceReturnsChatsWithNullProperties_ShouldHandleGracefully()
    {
        // Arrange
        var accountId = "account-123";
        var query = new GetChatsQuery(accountId);
        var cancellationToken = CancellationToken.None;

        var serviceChatDtos = new List<ChatDto>
        {
            new() { Id = "chat-1", ChatName = null, PhotoUrl = null, LastMessageId = null, Image = null, AccountIds = null },
            new() { Id = "chat-2", ChatName = "Valid Chat", UnreadCount = 5 }
        }.AsReadOnly();

        var mappedChatDtos = new List<ChatDto>
        {
            new() { Id = "chat-1", ChatName = null, PhotoUrl = null, LastMessageId = null, Image = null, AccountIds = null },
            new() { Id = "chat-2", ChatName = "Valid Chat", UnreadCount = 5 }
        };

        _chatServiceMock
            .Setup(x => x.GetChatsAsync(accountId, cancellationToken))
            .ReturnsAsync(serviceChatDtos);

        _mapperMock
            .Setup(x => x.Map<List<ChatDto>>(serviceChatDtos))
            .Returns(mappedChatDtos);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().Id.Should().Be("chat-1");
        result.First().ChatName.Should().BeNull();
        result.First().PhotoUrl.Should().BeNull();
        result.First().LastMessageId.Should().BeNull();
        result.First().Image.Should().BeNull();
        result.First().AccountIds.Should().BeNull();
    }
}
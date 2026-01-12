using AutoMapper;
using FluentAssertions;
using LetsTalk.Server.API.Core.Services;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using Moq;

namespace LetsTalk.Server.API.Core.Tests.Services;

[TestFixture]
public class ChatServiceTests
{
    private Mock<IChatAgnosticService> _chatAgnosticServiceMock;
    private Mock<IMapper> _mapperMock;
    private ChatService _chatService;

    [SetUp]
    public void SetUp()
    {
        _chatAgnosticServiceMock = new Mock<IChatAgnosticService>();
        _mapperMock = new Mock<IMapper>();
        _chatService = new ChatService(_chatAgnosticServiceMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GetChatsAsync_ShouldReturnMappedChats_WhenChatsExist()
    {
        // Arrange
        var accountId = "account123";
        var cancellationToken = CancellationToken.None;
        var serviceModels = new List<ChatServiceModel>
        {
            new() { Id = "chat1", ChatName = "Chat 1" },
            new() { Id = "chat2", ChatName = "Chat 2" }
        };
        var expectedDtos = new List<ChatDto>
        {
            new() { Id = "chat1", ChatName = "Chat 1" },
            new() { Id = "chat2", ChatName = "Chat 2" }
        };

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatsAsync(accountId, cancellationToken))
            .ReturnsAsync(serviceModels);
        _mapperMock
            .Setup(x => x.Map<List<ChatDto>>(serviceModels))
            .Returns(expectedDtos);

        // Act
        var result = await _chatService.GetChatsAsync(accountId, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedDtos);
        _chatAgnosticServiceMock.Verify(x => x.GetChatsAsync(accountId, cancellationToken), Times.Once);
        _mapperMock.Verify(x => x.Map<List<ChatDto>>(serviceModels), Times.Once);
    }

    [Test]
    public async Task GetChatsAsync_ShouldReturnEmptyList_WhenNoChatsExist()
    {
        // Arrange
        var accountId = "account123";
        var cancellationToken = CancellationToken.None;
        var emptyServiceModels = new List<ChatServiceModel>();
        var emptyDtos = new List<ChatDto>();

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatsAsync(accountId, cancellationToken))
            .ReturnsAsync(emptyServiceModels);
        _mapperMock
            .Setup(x => x.Map<List<ChatDto>>(emptyServiceModels))
            .Returns(emptyDtos);

        // Act
        var result = await _chatService.GetChatsAsync(accountId, cancellationToken);

        // Assert
        result.Should().BeEmpty();
        _chatAgnosticServiceMock.Verify(x => x.GetChatsAsync(accountId, cancellationToken), Times.Once);
        _mapperMock.Verify(x => x.Map<List<ChatDto>>(emptyServiceModels), Times.Once);
    }

    [Test]
    public async Task IsChatIdValidAsync_ShouldReturnTrue_WhenChatIdIsValid()
    {
        // Arrange
        var chatId = "chat123";
        var cancellationToken = CancellationToken.None;

        _chatAgnosticServiceMock
            .Setup(x => x.IsChatIdValidAsync(chatId, cancellationToken))
            .ReturnsAsync(true);

        // Act
        var result = await _chatService.IsChatIdValidAsync(chatId, cancellationToken);

        // Assert
        result.Should().BeTrue();
        _chatAgnosticServiceMock.Verify(x => x.IsChatIdValidAsync(chatId, cancellationToken), Times.Once);
    }

    [Test]
    public async Task IsChatIdValidAsync_ShouldReturnFalse_WhenChatIdIsInvalid()
    {
        // Arrange
        var chatId = "invalidChat";
        var cancellationToken = CancellationToken.None;

        _chatAgnosticServiceMock
            .Setup(x => x.IsChatIdValidAsync(chatId, cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await _chatService.IsChatIdValidAsync(chatId, cancellationToken);

        // Assert
        result.Should().BeFalse();
        _chatAgnosticServiceMock.Verify(x => x.IsChatIdValidAsync(chatId, cancellationToken), Times.Once);
    }

    [Test]
    public async Task IsAccountChatMemberAsync_ShouldReturnTrue_WhenAccountIsChatMember()
    {
        // Arrange
        var chatId = "chat123";
        var accountId = "account123";
        var cancellationToken = CancellationToken.None;

        _chatAgnosticServiceMock
            .Setup(x => x.IsAccountChatMemberAsync(chatId, accountId, cancellationToken))
            .ReturnsAsync(true);

        // Act
        var result = await _chatService.IsAccountChatMemberAsync(chatId, accountId, cancellationToken);

        // Assert
        result.Should().BeTrue();
        _chatAgnosticServiceMock.Verify(x => x.IsAccountChatMemberAsync(chatId, accountId, cancellationToken), Times.Once);
    }

    [Test]
    public async Task IsAccountChatMemberAsync_ShouldReturnFalse_WhenAccountIsNotChatMember()
    {
        // Arrange
        var chatId = "chat123";
        var accountId = "account123";
        var cancellationToken = CancellationToken.None;

        _chatAgnosticServiceMock
            .Setup(x => x.IsAccountChatMemberAsync(chatId, accountId, cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = await _chatService.IsAccountChatMemberAsync(chatId, accountId, cancellationToken);

        // Assert
        result.Should().BeFalse();
        _chatAgnosticServiceMock.Verify(x => x.IsAccountChatMemberAsync(chatId, accountId, cancellationToken), Times.Once);
    }

    [Test]
    public async Task IsAccountChatMemberAsync_ShouldUseDefaultCancellationToken_WhenNotProvided()
    {
        // Arrange
        var chatId = "chat123";
        var accountId = "account123";

        _chatAgnosticServiceMock
            .Setup(x => x.IsAccountChatMemberAsync(chatId, accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _chatService.IsAccountChatMemberAsync(chatId, accountId);

        // Assert
        _chatAgnosticServiceMock.Verify(x => x.IsAccountChatMemberAsync(chatId, accountId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
using FluentAssertions;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.API.Core.Features.Chat.Commands.CreateIndividualChat;
using LetsTalk.Server.API.Models.Chat;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using Moq;

namespace LetsTalk.Server.API.Core.Tests.Handlers;

[TestFixture]
public class CreateIndividualChatCommandHandlerTests
{
    private Mock<IChatAgnosticService> _chatAgnosticServiceMock;
    private Mock<IAccountAgnosticService> _accountAgnosticServiceMock;
    private Mock<IChatCacheManager> _chatCacheManagerMock;
    private CreateIndividualChatCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _chatAgnosticServiceMock = new Mock<IChatAgnosticService>();
        _accountAgnosticServiceMock = new Mock<IAccountAgnosticService>();
        _chatCacheManagerMock = new Mock<IChatCacheManager>();
        _handler = new CreateIndividualChatCommandHandler(
            _chatAgnosticServiceMock.Object,
            _accountAgnosticServiceMock.Object,
            _chatCacheManagerMock.Object);
    }

    [Test]
    public async Task Handle_WhenValidRequest_ShouldCreateChatAndReturnResponse()
    {
        // Arrange
        var invitingAccountId = "inviting-account-123";
        var accountId = "account-456";
        var chatId = "chat-789";
        var command = new CreateIndividualChatCommand(invitingAccountId, accountId);
        var cancellationToken = CancellationToken.None;

        _accountAgnosticServiceMock
            .Setup(x => x.IsAccountIdValidAsync(accountId, cancellationToken))
            .ReturnsAsync(true);

        _chatAgnosticServiceMock
            .Setup(x => x.CreateIndividualChatAsync(
                It.Is<IEnumerable<string>>(ids => ids.Contains(invitingAccountId) && ids.Contains(accountId)),
                cancellationToken))
            .ReturnsAsync(chatId);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Dto.Should().NotBeNull();
        result.Dto!.Id.Should().Be(chatId);
    }

    [Test]
    public async Task Handle_WhenValidRequest_ShouldCallCreateIndividualChatWithCorrectAccountIds()
    {
        // Arrange
        var invitingAccountId = "inviting-account-123";
        var accountId = "account-456";
        var chatId = "chat-789";
        var command = new CreateIndividualChatCommand(invitingAccountId, accountId);
        var cancellationToken = CancellationToken.None;

        _accountAgnosticServiceMock
            .Setup(x => x.IsAccountIdValidAsync(accountId, cancellationToken))
            .ReturnsAsync(true);

        _chatAgnosticServiceMock
            .Setup(x => x.CreateIndividualChatAsync(It.IsAny<IEnumerable<string>>(), cancellationToken))
            .ReturnsAsync(chatId);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _chatAgnosticServiceMock.Verify(
            x => x.CreateIndividualChatAsync(
                It.Is<IEnumerable<string>>(ids => 
                    ids.Count() == 2 &&
                    ids.Contains(invitingAccountId) &&
                    ids.Contains(accountId)),
                cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenValidRequest_ShouldClearCacheForBothAccounts()
    {
        // Arrange
        var invitingAccountId = "inviting-account-123";
        var accountId = "account-456";
        var chatId = "chat-789";
        var command = new CreateIndividualChatCommand(invitingAccountId, accountId);
        var cancellationToken = CancellationToken.None;

        _accountAgnosticServiceMock
            .Setup(x => x.IsAccountIdValidAsync(accountId, cancellationToken))
            .ReturnsAsync(true);

        _chatAgnosticServiceMock
            .Setup(x => x.CreateIndividualChatAsync(It.IsAny<IEnumerable<string>>(), cancellationToken))
            .ReturnsAsync(chatId);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _chatCacheManagerMock.Verify(
            x => x.ClearAsync(invitingAccountId),
            Times.Once);

        _chatCacheManagerMock.Verify(
            x => x.ClearAsync(accountId),
            Times.Once);
    }

    [Test]
    public void Handle_WhenValidationFails_ShouldThrowBadRequestException()
    {
        // Arrange
        var invitingAccountId = "inviting-account-123";
        var accountId = "invalid-account";
        var command = new CreateIndividualChatCommand(invitingAccountId, accountId);
        var cancellationToken = CancellationToken.None;

        _accountAgnosticServiceMock
            .Setup(x => x.IsAccountIdValidAsync(accountId, cancellationToken))
            .ReturnsAsync(false);

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public async Task Handle_WhenValidationFails_ShouldNotCreateChatOrClearCache()
    {
        // Arrange
        var invitingAccountId = "inviting-account-123";
        var accountId = "invalid-account";
        var command = new CreateIndividualChatCommand(invitingAccountId, accountId);
        var cancellationToken = CancellationToken.None;

        _accountAgnosticServiceMock
            .Setup(x => x.IsAccountIdValidAsync(accountId, cancellationToken))
            .ReturnsAsync(false);

        // Act & Assert
        try
        {
            await _handler.Handle(command, cancellationToken);
        }
        catch (BadRequestException)
        {
            // Expected exception
        }

        // Verify that chat creation and cache clearing were not called
        _chatAgnosticServiceMock.Verify(
            x => x.CreateIndividualChatAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _chatCacheManagerMock.Verify(
            x => x.ClearAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Test]
    public async Task Handle_WhenAccountIdIsNull_ShouldThrowBadRequestException()
    {
        // Arrange
        var invitingAccountId = "inviting-account-123";
        var command = new CreateIndividualChatCommand(invitingAccountId, null!);
        var cancellationToken = CancellationToken.None;

        _accountAgnosticServiceMock
            .Setup(x => x.IsAccountIdValidAsync(It.IsAny<string>(), cancellationToken))
            .ReturnsAsync(false);

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public async Task Handle_WhenAccountIdIsEmpty_ShouldThrowBadRequestException()
    {
        // Arrange
        var invitingAccountId = "inviting-account-123";
        var accountId = "";
        var command = new CreateIndividualChatCommand(invitingAccountId, accountId);
        var cancellationToken = CancellationToken.None;

        _accountAgnosticServiceMock
            .Setup(x => x.IsAccountIdValidAsync(accountId, cancellationToken))
            .ReturnsAsync(false);

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public async Task Handle_WhenAccountIdIsWhitespace_ShouldThrowBadRequestException()
    {
        // Arrange
        var invitingAccountId = "inviting-account-123";
        var accountId = "   ";
        var command = new CreateIndividualChatCommand(invitingAccountId, accountId);
        var cancellationToken = CancellationToken.None;

        _accountAgnosticServiceMock
            .Setup(x => x.IsAccountIdValidAsync(accountId, cancellationToken))
            .ReturnsAsync(false);

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public async Task Handle_WhenCancellationTokenProvided_ShouldPassItToAllServices()
    {
        // Arrange
        var invitingAccountId = "inviting-account-123";
        var accountId = "account-456";
        var chatId = "chat-789";
        var command = new CreateIndividualChatCommand(invitingAccountId, accountId);
        var cancellationToken = new CancellationToken(false);

        _accountAgnosticServiceMock
            .Setup(x => x.IsAccountIdValidAsync(accountId, cancellationToken))
            .ReturnsAsync(true);

        _chatAgnosticServiceMock
            .Setup(x => x.CreateIndividualChatAsync(It.IsAny<IEnumerable<string>>(), cancellationToken))
            .ReturnsAsync(chatId);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _accountAgnosticServiceMock.Verify(
            x => x.IsAccountIdValidAsync(accountId, cancellationToken),
            Times.Once);

        _chatAgnosticServiceMock.Verify(
            x => x.CreateIndividualChatAsync(It.IsAny<IEnumerable<string>>(), cancellationToken),
            Times.Once);
    }

    [Test]
    public void Handle_WhenChatAgnosticServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var invitingAccountId = "inviting-account-123";
        var accountId = "account-456";
        var command = new CreateIndividualChatCommand(invitingAccountId, accountId);
        var cancellationToken = CancellationToken.None;

        _accountAgnosticServiceMock
            .Setup(x => x.IsAccountIdValidAsync(accountId, cancellationToken))
            .ReturnsAsync(true);

        _chatAgnosticServiceMock
            .Setup(x => x.CreateIndividualChatAsync(It.IsAny<IEnumerable<string>>(), cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Chat service error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public void Handle_WhenAccountAgnosticServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var invitingAccountId = "inviting-account-123";
        var accountId = "account-456";
        var command = new CreateIndividualChatCommand(invitingAccountId, accountId);
        var cancellationToken = CancellationToken.None;

        _accountAgnosticServiceMock
            .Setup(x => x.IsAccountIdValidAsync(accountId, cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Account service error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public void Handle_WhenChatCacheManagerThrowsException_ShouldPropagateException()
    {
        // Arrange
        var invitingAccountId = "inviting-account-123";
        var accountId = "account-456";
        var chatId = "chat-789";
        var command = new CreateIndividualChatCommand(invitingAccountId, accountId);
        var cancellationToken = CancellationToken.None;

        _accountAgnosticServiceMock
            .Setup(x => x.IsAccountIdValidAsync(accountId, cancellationToken))
            .ReturnsAsync(true);

        _chatAgnosticServiceMock
            .Setup(x => x.CreateIndividualChatAsync(It.IsAny<IEnumerable<string>>(), cancellationToken))
            .ReturnsAsync(chatId);

        _chatCacheManagerMock
            .Setup(x => x.ClearAsync(invitingAccountId))
            .ThrowsAsync(new InvalidOperationException("Cache manager error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public async Task Handle_WhenSameAccountIds_ShouldStillCreateChat()
    {
        // Arrange
        var accountId = "same-account-123";
        var chatId = "chat-789";
        var command = new CreateIndividualChatCommand(accountId, accountId);
        var cancellationToken = CancellationToken.None;

        _accountAgnosticServiceMock
            .Setup(x => x.IsAccountIdValidAsync(accountId, cancellationToken))
            .ReturnsAsync(true);

        _chatAgnosticServiceMock
            .Setup(x => x.CreateIndividualChatAsync(It.IsAny<IEnumerable<string>>(), cancellationToken))
            .ReturnsAsync(chatId);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Dto.Should().NotBeNull();
        result.Dto!.Id.Should().Be(chatId);

        // Verify that both account IDs are passed (even if they're the same)
        _chatAgnosticServiceMock.Verify(
            x => x.CreateIndividualChatAsync(
                It.Is<IEnumerable<string>>(ids => ids.Count() == 2 && ids.All(id => id == accountId)),
                cancellationToken),
            Times.Once);

        // Cache should still be cleared (even if it's the same account, it might be called twice)
        _chatCacheManagerMock.Verify(
            x => x.ClearAsync(accountId),
            Times.Exactly(2));
    }

    [Test]
    public async Task Handle_WhenValidRequest_ShouldFollowCompleteWorkflow()
    {
        // Arrange
        var invitingAccountId = "inviting-account-123";
        var accountId = "account-456";
        var chatId = "chat-789";
        var command = new CreateIndividualChatCommand(invitingAccountId, accountId);
        var cancellationToken = CancellationToken.None;

        _accountAgnosticServiceMock
            .Setup(x => x.IsAccountIdValidAsync(accountId, cancellationToken))
            .ReturnsAsync(true);

        _chatAgnosticServiceMock
            .Setup(x => x.CreateIndividualChatAsync(It.IsAny<IEnumerable<string>>(), cancellationToken))
            .ReturnsAsync(chatId);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Dto.Should().NotBeNull();
        result.Dto!.Id.Should().Be(chatId);

        // Verify the complete workflow
        _accountAgnosticServiceMock.Verify(
            x => x.IsAccountIdValidAsync(accountId, cancellationToken),
            Times.Once);

        _chatAgnosticServiceMock.Verify(
            x => x.CreateIndividualChatAsync(
                It.Is<IEnumerable<string>>(ids => 
                    ids.Contains(invitingAccountId) && ids.Contains(accountId)),
                cancellationToken),
            Times.Once);

        _chatCacheManagerMock.Verify(
            x => x.ClearAsync(invitingAccountId),
            Times.Once);

        _chatCacheManagerMock.Verify(
            x => x.ClearAsync(accountId),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenValidationPassesButAccountServiceReturnsTrue_ShouldCreateChat()
    {
        // Arrange
        var invitingAccountId = "inviting-account-123";
        var accountId = "valid-account-456";
        var chatId = "chat-789";
        var command = new CreateIndividualChatCommand(invitingAccountId, accountId);
        var cancellationToken = CancellationToken.None;

        _accountAgnosticServiceMock
            .Setup(x => x.IsAccountIdValidAsync(accountId, cancellationToken))
            .ReturnsAsync(true);

        _chatAgnosticServiceMock
            .Setup(x => x.CreateIndividualChatAsync(It.IsAny<IEnumerable<string>>(), cancellationToken))
            .ReturnsAsync(chatId);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Dto.Should().NotBeNull();
        result.Dto!.Id.Should().Be(chatId);

        // Verify validation was called
        _accountAgnosticServiceMock.Verify(
            x => x.IsAccountIdValidAsync(accountId, cancellationToken),
            Times.Once);
    }
}
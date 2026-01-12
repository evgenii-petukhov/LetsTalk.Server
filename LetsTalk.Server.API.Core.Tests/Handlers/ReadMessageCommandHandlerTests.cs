using FluentAssertions;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.API.Core.Features.Message.Commands.ReadMessage;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using MediatR;
using Moq;
using NUnit.Framework;

namespace LetsTalk.Server.API.Core.Tests.Handlers;

[TestFixture]
public class ReadMessageCommandHandlerTests
{
    private Mock<IMessageAgnosticService> _messageAgnosticServiceMock;
    private Mock<IChatCacheManager> _chatCacheManagerMock;
    private ReadMessageCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _messageAgnosticServiceMock = new Mock<IMessageAgnosticService>();
        _chatCacheManagerMock = new Mock<IChatCacheManager>();
        
        _handler = new ReadMessageCommandHandler(
            _messageAgnosticServiceMock.Object,
            _chatCacheManagerMock.Object);
    }

    [Test]
    public async Task Handle_WhenValidRequest_ShouldMarkMessageAsReadAndClearCache()
    {
        // Arrange
        var command = new ReadMessageCommand
        {
            ChatId = "chat-123",
            AccountId = "account-456",
            MessageId = "message-789"
        };
        var cancellationToken = CancellationToken.None;

        _messageAgnosticServiceMock
            .Setup(x => x.MarkAsReadAsync("chat-123", "account-456", "message-789", cancellationToken))
            .Returns(Task.CompletedTask);

        _chatCacheManagerMock
            .Setup(x => x.ClearAsync("account-456"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.MarkAsReadAsync("chat-123", "account-456", "message-789", cancellationToken),
            Times.Once);

        _chatCacheManagerMock.Verify(
            x => x.ClearAsync("account-456"),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenCancellationTokenProvided_ShouldPassItToServices()
    {
        // Arrange
        var command = new ReadMessageCommand
        {
            ChatId = "chat-123",
            AccountId = "account-456",
            MessageId = "message-789"
        };
        var cancellationToken = new CancellationToken(false);

        _messageAgnosticServiceMock
            .Setup(x => x.MarkAsReadAsync("chat-123", "account-456", "message-789", cancellationToken))
            .Returns(Task.CompletedTask);

        _chatCacheManagerMock
            .Setup(x => x.ClearAsync("account-456"))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _messageAgnosticServiceMock.Verify(
            x => x.MarkAsReadAsync("chat-123", "account-456", "message-789", cancellationToken),
            Times.Once);

        _chatCacheManagerMock.Verify(
            x => x.ClearAsync("account-456"),
            Times.Once);
    }

    [Test]
    public void Handle_WhenMessageServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new ReadMessageCommand
        {
            ChatId = "chat-123",
            AccountId = "account-456",
            MessageId = "message-789"
        };
        var cancellationToken = CancellationToken.None;

        _messageAgnosticServiceMock
            .Setup(x => x.MarkAsReadAsync("chat-123", "account-456", "message-789", cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Message service error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));

        _messageAgnosticServiceMock.Verify(
            x => x.MarkAsReadAsync("chat-123", "account-456", "message-789", cancellationToken),
            Times.Once);

        _chatCacheManagerMock.Verify(
            x => x.ClearAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Test]
    public void Handle_WhenCacheManagerThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new ReadMessageCommand
        {
            ChatId = "chat-123",
            AccountId = "account-456",
            MessageId = "message-789"
        };
        var cancellationToken = CancellationToken.None;

        _messageAgnosticServiceMock
            .Setup(x => x.MarkAsReadAsync("chat-123", "account-456", "message-789", cancellationToken))
            .Returns(Task.CompletedTask);

        _chatCacheManagerMock
            .Setup(x => x.ClearAsync("account-456"))
            .ThrowsAsync(new InvalidOperationException("Cache clearing failed"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));

        _messageAgnosticServiceMock.Verify(
            x => x.MarkAsReadAsync("chat-123", "account-456", "message-789", cancellationToken),
            Times.Once);

        _chatCacheManagerMock.Verify(
            x => x.ClearAsync("account-456"),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenNullChatId_ShouldPassNullToService()
    {
        // Arrange
        var command = new ReadMessageCommand
        {
            ChatId = null,
            AccountId = "account-456",
            MessageId = "message-789"
        };
        var cancellationToken = CancellationToken.None;

        _messageAgnosticServiceMock
            .Setup(x => x.MarkAsReadAsync(null!, "account-456", "message-789", cancellationToken))
            .Returns(Task.CompletedTask);

        _chatCacheManagerMock
            .Setup(x => x.ClearAsync("account-456"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.MarkAsReadAsync(null!, "account-456", "message-789", cancellationToken),
            Times.Once);

        _chatCacheManagerMock.Verify(
            x => x.ClearAsync("account-456"),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenNullAccountId_ShouldPassNullToService()
    {
        // Arrange
        var command = new ReadMessageCommand
        {
            ChatId = "chat-123",
            AccountId = null,
            MessageId = "message-789"
        };
        var cancellationToken = CancellationToken.None;

        _messageAgnosticServiceMock
            .Setup(x => x.MarkAsReadAsync("chat-123", null!, "message-789", cancellationToken))
            .Returns(Task.CompletedTask);

        _chatCacheManagerMock
            .Setup(x => x.ClearAsync(null!))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.MarkAsReadAsync("chat-123", null!, "message-789", cancellationToken),
            Times.Once);

        _chatCacheManagerMock.Verify(
            x => x.ClearAsync(null!),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenNullMessageId_ShouldPassNullToService()
    {
        // Arrange
        var command = new ReadMessageCommand
        {
            ChatId = "chat-123",
            AccountId = "account-456",
            MessageId = null
        };
        var cancellationToken = CancellationToken.None;

        _messageAgnosticServiceMock
            .Setup(x => x.MarkAsReadAsync("chat-123", "account-456", null!, cancellationToken))
            .Returns(Task.CompletedTask);

        _chatCacheManagerMock
            .Setup(x => x.ClearAsync("account-456"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.MarkAsReadAsync("chat-123", "account-456", null!, cancellationToken),
            Times.Once);

        _chatCacheManagerMock.Verify(
            x => x.ClearAsync("account-456"),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenAllPropertiesNull_ShouldPassNullsToServices()
    {
        // Arrange
        var command = new ReadMessageCommand
        {
            ChatId = null,
            AccountId = null,
            MessageId = null
        };
        var cancellationToken = CancellationToken.None;

        _messageAgnosticServiceMock
            .Setup(x => x.MarkAsReadAsync(null!, null!, null!, cancellationToken))
            .Returns(Task.CompletedTask);

        _chatCacheManagerMock
            .Setup(x => x.ClearAsync(null!))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.MarkAsReadAsync(null!, null!, null!, cancellationToken),
            Times.Once);

        _chatCacheManagerMock.Verify(
            x => x.ClearAsync(null!),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenEmptyStringProperties_ShouldPassEmptyStringsToServices()
    {
        // Arrange
        var command = new ReadMessageCommand
        {
            ChatId = "",
            AccountId = "",
            MessageId = ""
        };
        var cancellationToken = CancellationToken.None;

        _messageAgnosticServiceMock
            .Setup(x => x.MarkAsReadAsync("", "", "", cancellationToken))
            .Returns(Task.CompletedTask);

        _chatCacheManagerMock
            .Setup(x => x.ClearAsync(""))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.MarkAsReadAsync("", "", "", cancellationToken),
            Times.Once);

        _chatCacheManagerMock.Verify(
            x => x.ClearAsync(""),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenWhitespaceProperties_ShouldPassWhitespaceToServices()
    {
        // Arrange
        var command = new ReadMessageCommand
        {
            ChatId = "   ",
            AccountId = "\t",
            MessageId = "\n"
        };
        var cancellationToken = CancellationToken.None;

        _messageAgnosticServiceMock
            .Setup(x => x.MarkAsReadAsync("   ", "\t", "\n", cancellationToken))
            .Returns(Task.CompletedTask);

        _chatCacheManagerMock
            .Setup(x => x.ClearAsync("\t"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.MarkAsReadAsync("   ", "\t", "\n", cancellationToken),
            Times.Once);

        _chatCacheManagerMock.Verify(
            x => x.ClearAsync("\t"),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenLongStringProperties_ShouldPassLongStringsToServices()
    {
        // Arrange
        var longChatId = new string('a', 1000);
        var longAccountId = new string('b', 1000);
        var longMessageId = new string('c', 1000);

        var command = new ReadMessageCommand
        {
            ChatId = longChatId,
            AccountId = longAccountId,
            MessageId = longMessageId
        };
        var cancellationToken = CancellationToken.None;

        _messageAgnosticServiceMock
            .Setup(x => x.MarkAsReadAsync(longChatId, longAccountId, longMessageId, cancellationToken))
            .Returns(Task.CompletedTask);

        _chatCacheManagerMock
            .Setup(x => x.ClearAsync(longAccountId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.MarkAsReadAsync(longChatId, longAccountId, longMessageId, cancellationToken),
            Times.Once);

        _chatCacheManagerMock.Verify(
            x => x.ClearAsync(longAccountId),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenSpecialCharacterProperties_ShouldPassSpecialCharactersToServices()
    {
        // Arrange
        var command = new ReadMessageCommand
        {
            ChatId = "chat-123!@#$%^&*()",
            AccountId = "account-456<>?:\"{}|",
            MessageId = "message-789[]\\;',./"
        };
        var cancellationToken = CancellationToken.None;

        _messageAgnosticServiceMock
            .Setup(x => x.MarkAsReadAsync("chat-123!@#$%^&*()", "account-456<>?:\"{}|", "message-789[]\\;',./", cancellationToken))
            .Returns(Task.CompletedTask);

        _chatCacheManagerMock
            .Setup(x => x.ClearAsync("account-456<>?:\"{}|"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.MarkAsReadAsync("chat-123!@#$%^&*()", "account-456<>?:\"{}|", "message-789[]\\;',./", cancellationToken),
            Times.Once);

        _chatCacheManagerMock.Verify(
            x => x.ClearAsync("account-456<>?:\"{}|"),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenUnicodeProperties_ShouldPassUnicodeToServices()
    {
        // Arrange
        var command = new ReadMessageCommand
        {
            ChatId = "chat-123-🚀",
            AccountId = "account-456-中文",
            MessageId = "message-789-العربية"
        };
        var cancellationToken = CancellationToken.None;

        _messageAgnosticServiceMock
            .Setup(x => x.MarkAsReadAsync("chat-123-🚀", "account-456-中文", "message-789-العربية", cancellationToken))
            .Returns(Task.CompletedTask);

        _chatCacheManagerMock
            .Setup(x => x.ClearAsync("account-456-中文"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.MarkAsReadAsync("chat-123-🚀", "account-456-中文", "message-789-العربية", cancellationToken),
            Times.Once);

        _chatCacheManagerMock.Verify(
            x => x.ClearAsync("account-456-中文"),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenExecutedMultipleTimes_ShouldCallServicesEachTime()
    {
        // Arrange
        var command = new ReadMessageCommand
        {
            ChatId = "chat-123",
            AccountId = "account-456",
            MessageId = "message-789"
        };
        var cancellationToken = CancellationToken.None;

        _messageAgnosticServiceMock
            .Setup(x => x.MarkAsReadAsync("chat-123", "account-456", "message-789", cancellationToken))
            .Returns(Task.CompletedTask);

        _chatCacheManagerMock
            .Setup(x => x.ClearAsync("account-456"))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, cancellationToken);
        await _handler.Handle(command, cancellationToken);
        await _handler.Handle(command, cancellationToken);

        // Assert
        _messageAgnosticServiceMock.Verify(
            x => x.MarkAsReadAsync("chat-123", "account-456", "message-789", cancellationToken),
            Times.Exactly(3));

        _chatCacheManagerMock.Verify(
            x => x.ClearAsync("account-456"),
            Times.Exactly(3));
    }

    [Test]
    public async Task Handle_WhenOperationsExecuteSequentially_ShouldMaintainOrder()
    {
        // Arrange
        var command = new ReadMessageCommand
        {
            ChatId = "chat-123",
            AccountId = "account-456",
            MessageId = "message-789"
        };
        var cancellationToken = CancellationToken.None;

        var callOrder = new List<string>();

        _messageAgnosticServiceMock
            .Setup(x => x.MarkAsReadAsync("chat-123", "account-456", "message-789", cancellationToken))
            .Returns(() =>
            {
                callOrder.Add("MarkAsRead");
                return Task.CompletedTask;
            });

        _chatCacheManagerMock
            .Setup(x => x.ClearAsync("account-456"))
            .Returns(() =>
            {
                callOrder.Add("ClearCache");
                return Task.CompletedTask;
            });

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        callOrder.Should().HaveCount(2);
        callOrder[0].Should().Be("MarkAsRead");
        callOrder[1].Should().Be("ClearCache");
    }

    [Test]
    public async Task Handle_WhenBothOperationsSucceed_ShouldReturnUnitValue()
    {
        // Arrange
        var command = new ReadMessageCommand
        {
            ChatId = "chat-123",
            AccountId = "account-456",
            MessageId = "message-789"
        };
        var cancellationToken = CancellationToken.None;

        _messageAgnosticServiceMock
            .Setup(x => x.MarkAsReadAsync("chat-123", "account-456", "message-789", cancellationToken))
            .Returns(Task.CompletedTask);

        _chatCacheManagerMock
            .Setup(x => x.ClearAsync("account-456"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);
        result.Should().NotBeNull();
    }
}
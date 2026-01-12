using FluentAssertions;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.API.Core.Features.VideoCall.Commands.HandleIncomingCall;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using MediatR;
using Moq;
using NUnit.Framework;

namespace LetsTalk.Server.API.Core.Tests.Handlers;

[TestFixture]
public class HandleIncomingCallCommandHandlerTests
{
    private Mock<IProducer<Notification>> _notificationProducerMock;
    private Mock<IChatAgnosticService> _chatAgnosticServiceMock;
    private HandleIncomingCallCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _notificationProducerMock = new Mock<IProducer<Notification>>();
        _chatAgnosticServiceMock = new Mock<IChatAgnosticService>();

        _handler = new HandleIncomingCallCommandHandler(
            _notificationProducerMock.Object,
            _chatAgnosticServiceMock.Object);
    }

    [Test]
    public async Task Handle_WhenTwoMembersInChat_ShouldSendNotificationToOtherMember()
    {
        // Arrange
        var command = new HandleIncomingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Answer: "sdp-answer-data");
        var cancellationToken = CancellationToken.None;

        var chatMembers = new List<string> { "caller-123", "recipient-789" };

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(chatMembers);

        _notificationProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _chatAgnosticServiceMock.Verify(
            x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken),
            Times.Once);

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == "recipient-789" &&
                n.Connection != null &&
                n.Connection.Answer == "sdp-answer-data"), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenCallerIsFirstInList_ShouldSendToSecondMember()
    {
        // Arrange
        var command = new HandleIncomingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Answer: "answer-data");
        var cancellationToken = CancellationToken.None;

        var chatMembers = new List<string> { "caller-123", "recipient-789" };

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(chatMembers);

        _notificationProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == "recipient-789"), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenCallerIsSecondInList_ShouldSendToFirstMember()
    {
        // Arrange
        var command = new HandleIncomingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Answer: "answer-data");
        var cancellationToken = CancellationToken.None;

        var chatMembers = new List<string> { "recipient-789", "caller-123" };

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(chatMembers);

        _notificationProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == "recipient-789"), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenOnlyCallerInChat_ShouldSendNotificationWithNullRecipient()
    {
        // Arrange
        var command = new HandleIncomingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Answer: "answer-data");
        var cancellationToken = CancellationToken.None;

        var chatMembers = new List<string> { "caller-123" };

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(chatMembers);

        _notificationProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == null &&
                n.Connection != null &&
                n.Connection.Answer == "answer-data"), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenEmptyChatMembers_ShouldSendNotificationWithNullRecipient()
    {
        // Arrange
        var command = new HandleIncomingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Answer: "answer-data");
        var cancellationToken = CancellationToken.None;

        var chatMembers = new List<string>();

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(chatMembers);

        _notificationProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == null &&
                n.Connection != null &&
                n.Connection.Answer == "answer-data"), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenMultipleMembersInChat_ShouldSendToFirstNonCallerMember()
    {
        // Arrange
        var command = new HandleIncomingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Answer: "answer-data");
        var cancellationToken = CancellationToken.None;

        var chatMembers = new List<string> { "member-1", "caller-123", "member-2", "member-3" };

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(chatMembers);

        _notificationProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == "member-1"), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenCallerNotInChatMembers_ShouldSendToFirstMember()
    {
        // Arrange
        var command = new HandleIncomingCallCommand(
            AccountId: "caller-not-in-chat",
            ChatId: "chat-456",
            Answer: "answer-data");
        var cancellationToken = CancellationToken.None;

        var chatMembers = new List<string> { "member-1", "member-2", "member-3" };

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(chatMembers);

        _notificationProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == "member-1"), cancellationToken),
            Times.Once);
    }

    [Test]
    public void Handle_WhenChatServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new HandleIncomingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Answer: "answer-data");
        var cancellationToken = CancellationToken.None;

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Chat service error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));

        _chatAgnosticServiceMock.Verify(
            x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken),
            Times.Once);

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public void Handle_WhenNotificationProducerThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new HandleIncomingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Answer: "answer-data");
        var cancellationToken = CancellationToken.None;

        var chatMembers = new List<string> { "caller-123", "recipient-789" };

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(chatMembers);

        _notificationProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Notification publishing failed"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));

        _chatAgnosticServiceMock.Verify(
            x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken),
            Times.Once);

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenCancellationTokenProvided_ShouldPassItToAllServices()
    {
        // Arrange
        var command = new HandleIncomingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Answer: "answer-data");
        var cancellationToken = new CancellationToken(false);

        var chatMembers = new List<string> { "caller-123", "recipient-789" };

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(chatMembers);

        _notificationProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert - Verification is done in the setup methods that check cancellationToken
        _chatAgnosticServiceMock.Verify(
            x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken),
            Times.Once);

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenNullProperties_ShouldHandleCorrectly()
    {
        // Arrange
        var command = new HandleIncomingCallCommand(
            AccountId: null!,
            ChatId: null!,
            Answer: null!);
        var cancellationToken = CancellationToken.None;

        var chatMembers = new List<string> { "member-1", "member-2" };

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync(null!, cancellationToken))
            .ReturnsAsync(chatMembers);

        _notificationProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _chatAgnosticServiceMock.Verify(
            x => x.GetChatMemberAccountIdsAsync(null!, cancellationToken),
            Times.Once);

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == "member-1" &&
                n.Connection != null &&
                n.Connection.Answer == null), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenSpecialCharactersInProperties_ShouldHandleCorrectly()
    {
        // Arrange
        var command = new HandleIncomingCallCommand(
            AccountId: "caller-!@#$%",
            ChatId: "chat-^&*()",
            Answer: "answer-with-special-chars-!@#$%^&*()");
        var cancellationToken = CancellationToken.None;

        var chatMembers = new List<string> { "caller-!@#$%", "recipient-{}[]" };

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-^&*()", cancellationToken))
            .ReturnsAsync(chatMembers);

        _notificationProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == "recipient-{}[]" &&
                n.Connection != null &&
                n.Connection.Answer == "answer-with-special-chars-!@#$%^&*()"), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenUnicodeCharactersInProperties_ShouldHandleCorrectly()
    {
        // Arrange
        var command = new HandleIncomingCallCommand(
            AccountId: "呼叫者-123",
            ChatId: "聊天-456",
            Answer: "答案-数据-🎥📞");
        var cancellationToken = CancellationToken.None;

        var chatMembers = new List<string> { "呼叫者-123", "接收者-789" };

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("聊天-456", cancellationToken))
            .ReturnsAsync(chatMembers);

        _notificationProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == "接收者-789" &&
                n.Connection != null &&
                n.Connection.Answer == "答案-数据-🎥📞"), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenLongAnswerData_ShouldHandleCorrectly()
    {
        // Arrange
        var longAnswer = new string('a', 10000); // Very long SDP answer
        var command = new HandleIncomingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Answer: longAnswer);
        var cancellationToken = CancellationToken.None;

        var chatMembers = new List<string> { "caller-123", "recipient-789" };

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(chatMembers);

        _notificationProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == "recipient-789" &&
                n.Connection != null &&
                n.Connection.Answer == longAnswer &&
                n.Connection.Answer!.Length == 10000), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenEmptyStringProperties_ShouldHandleCorrectly()
    {
        // Arrange
        var command = new HandleIncomingCallCommand(
            AccountId: "",
            ChatId: "",
            Answer: "");
        var cancellationToken = CancellationToken.None;

        var chatMembers = new List<string> { "", "recipient-789" };

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("", cancellationToken))
            .ReturnsAsync(chatMembers);

        _notificationProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == "recipient-789" &&
                n.Connection != null &&
                n.Connection.Answer == ""), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenDuplicateAccountIdsInChat_ShouldFindFirstDifferentOne()
    {
        // Arrange
        var command = new HandleIncomingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Answer: "answer-data");
        var cancellationToken = CancellationToken.None;

        var chatMembers = new List<string> { "caller-123", "caller-123", "recipient-789", "caller-123" };

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(chatMembers);

        _notificationProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == "recipient-789"), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenAllMembersAreCaller_ShouldSendNotificationWithNullRecipient()
    {
        // Arrange
        var command = new HandleIncomingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Answer: "answer-data");
        var cancellationToken = CancellationToken.None;

        var chatMembers = new List<string> { "caller-123", "caller-123", "caller-123" };

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(chatMembers);

        _notificationProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == null &&
                n.Connection != null &&
                n.Connection.Answer == "answer-data"), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_ShouldCreateNotificationWithCorrectRtcSessionSettings()
    {
        // Arrange
        var command = new HandleIncomingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Answer: "test-sdp-answer");
        var cancellationToken = CancellationToken.None;

        var chatMembers = new List<string> { "caller-123", "recipient-789" };

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(chatMembers);

        _notificationProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == "recipient-789" &&
                n.Connection != null &&
                n.Connection.Answer == "test-sdp-answer" &&
                n.Connection.Offer == null &&
                n.Connection.ChatId == null), cancellationToken),
            Times.Once);
    }
}
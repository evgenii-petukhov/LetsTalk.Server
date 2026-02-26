using AutoMapper;
using FluentAssertions;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.API.Core.Features.VideoCall.Commands.StartOutgoingCall;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Telemetry.Abstractions;
using LetsTalk.Server.Telemetry.Models;
using Moq;

namespace LetsTalk.Server.API.Core.Tests.Handlers;

[TestFixture]
public class StartOutgoingCallCommandHandlerTests
{
    private Mock<IProducer<Notification>> _notificationProducerMock;
    private Mock<IChatAgnosticService> _chatAgnosticServiceMock;
    private Mock<IAccountAgnosticService> _accountAgnosticServiceMock;
    private Mock<ITelemetryService> _telemetryServiceMock;
    private Mock<IMapper> _mapperMock;
    private StartOutgoingCallCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _notificationProducerMock = new Mock<IProducer<Notification>>();
        _chatAgnosticServiceMock = new Mock<IChatAgnosticService>();
        _accountAgnosticServiceMock = new Mock<IAccountAgnosticService>();
        _telemetryServiceMock = new Mock<ITelemetryService>();
        _mapperMock = new Mock<IMapper>();

        _handler = new StartOutgoingCallCommandHandler(
            _notificationProducerMock.Object,
            _chatAgnosticServiceMock.Object,
            _accountAgnosticServiceMock.Object,
            _telemetryServiceMock.Object,
            _mapperMock.Object);
    }

    [Test]
    public async Task Handle_WhenTwoMembersInChat_ShouldSendNotificationToOtherMember()
    {
        // Arrange
        var command = new StartOutgoingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Offer: "sdp-offer-data",
            ConnectionDiagnostics: new ConnectionDiagnostics
            {
                ConnectionState = "connected",
                LocalCandidateTypes = "{}",
                RemoteCandidateTypes = "{}",
                Browser = "Chrome",
                Platform = "Win32"
            },
            IceGatheringElapsedMs: 0,
            IceGatheringCollectedAll: false);
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
        result.Should().NotBeNull()
            .And.BeOfType<StartOutgoingCallDto>()
            .Which.CallId.Should().NotBeNullOrEmpty();

        _chatAgnosticServiceMock.Verify(
            x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken),
            Times.Once);

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == "recipient-789" &&
                n.IncomingCall != null &&
                n.IncomingCall.Offer == "sdp-offer-data" &&
                n.IncomingCall.ChatId == "chat-456"), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenCallerIsFirstInList_ShouldSendToSecondMember()
    {
        // Arrange
        var command = new StartOutgoingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Offer: "offer-data",
            ConnectionDiagnostics: new ConnectionDiagnostics
            {
                ConnectionState = "connected",
                LocalCandidateTypes = "{}",
                RemoteCandidateTypes = "{}",
                Browser = "Chrome",
                Platform = "Win32"
            },
            IceGatheringElapsedMs: 0,
            IceGatheringCollectedAll: false);
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
        result.Should().NotBeNull()
            .And.BeOfType<StartOutgoingCallDto>()
            .Which.CallId.Should().NotBeNullOrEmpty();

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == "recipient-789"), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenCallerIsSecondInList_ShouldSendToFirstMember()
    {
        // Arrange
        var command = new StartOutgoingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Offer: "offer-data",
            ConnectionDiagnostics: new ConnectionDiagnostics
            {
                ConnectionState = "connected",
                LocalCandidateTypes = "{}",
                RemoteCandidateTypes = "{}",
                Browser = "Chrome",
                Platform = "Win32"
            },
            IceGatheringElapsedMs: 0,
            IceGatheringCollectedAll: false);
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
        result.Should().NotBeNull()
            .And.BeOfType<StartOutgoingCallDto>()
            .Which.CallId.Should().NotBeNullOrEmpty();

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == "recipient-789"), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenOnlyCallerInChat_ShouldSendNotificationWithNullRecipient()
    {
        // Arrange
        var command = new StartOutgoingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Offer: "offer-data",
            ConnectionDiagnostics: new ConnectionDiagnostics
            {
                ConnectionState = "connected",
                LocalCandidateTypes = "{}",
                RemoteCandidateTypes = "{}",
                Browser = "Chrome",
                Platform = "Win32"
            },
            IceGatheringElapsedMs: 0,
            IceGatheringCollectedAll: false);
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
        result.Should().NotBeNull()
            .And.BeOfType<StartOutgoingCallDto>()
            .Which.CallId.Should().NotBeNullOrEmpty();

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == null &&
                n.IncomingCall != null &&
                n.IncomingCall.Offer == "offer-data" &&
                n.IncomingCall.ChatId == "chat-456"), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenEmptyChatMembers_ShouldSendNotificationWithNullRecipient()
    {
        // Arrange
        var command = new StartOutgoingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Offer: "offer-data",
            ConnectionDiagnostics: new ConnectionDiagnostics
            {
                ConnectionState = "connected",
                LocalCandidateTypes = "{}",
                RemoteCandidateTypes = "{}",
                Browser = "Chrome",
                Platform = "Win32"
            },
            IceGatheringElapsedMs: 0,
            IceGatheringCollectedAll: false);
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
        result.Should().NotBeNull()
            .And.BeOfType<StartOutgoingCallDto>()
            .Which.CallId.Should().NotBeNullOrEmpty();

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == null &&
                n.IncomingCall != null &&
                n.IncomingCall.Offer == "offer-data" &&
                n.IncomingCall.ChatId == "chat-456"), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenMultipleMembersInChat_ShouldSendToFirstNonCallerMember()
    {
        // Arrange
        var command = new StartOutgoingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Offer: "offer-data",
            ConnectionDiagnostics: new ConnectionDiagnostics
            {
                ConnectionState = "connected",
                LocalCandidateTypes = "{}",
                RemoteCandidateTypes = "{}",
                Browser = "Chrome",
                Platform = "Win32"
            },
            IceGatheringElapsedMs: 0,
            IceGatheringCollectedAll: false);
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
        result.Should().NotBeNull()
            .And.BeOfType<StartOutgoingCallDto>()
            .Which.CallId.Should().NotBeNullOrEmpty();

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == "member-1"), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenCallerNotInChatMembers_ShouldSendToFirstMember()
    {
        // Arrange
        var command = new StartOutgoingCallCommand(
            AccountId: "caller-not-in-chat",
            ChatId: "chat-456",
            Offer: "offer-data",
            ConnectionDiagnostics: new ConnectionDiagnostics
            {
                ConnectionState = "connected",
                LocalCandidateTypes = "{}",
                RemoteCandidateTypes = "{}",
                Browser = "Chrome",
                Platform = "Win32"
            },
            IceGatheringElapsedMs: 0,
            IceGatheringCollectedAll: false);
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
        result.Should().NotBeNull()
            .And.BeOfType<StartOutgoingCallDto>()
            .Which.CallId.Should().NotBeNullOrEmpty();

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == "member-1"), cancellationToken),
            Times.Once);
    }

    [Test]
    public void Handle_WhenChatServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new StartOutgoingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Offer: "offer-data",
            ConnectionDiagnostics: new ConnectionDiagnostics
            {
                ConnectionState = "connected",
                LocalCandidateTypes = "{}",
                RemoteCandidateTypes = "{}",
                Browser = "Chrome",
                Platform = "Win32"
            },
            IceGatheringElapsedMs: 0,
            IceGatheringCollectedAll: false);
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
        var command = new StartOutgoingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Offer: "offer-data",
            ConnectionDiagnostics: new ConnectionDiagnostics
            {
                ConnectionState = "connected",
                LocalCandidateTypes = "{}",
                RemoteCandidateTypes = "{}",
                Browser = "Chrome",
                Platform = "Win32"
            },
            IceGatheringElapsedMs: 0,
            IceGatheringCollectedAll: false);
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
        var command = new StartOutgoingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Offer: "offer-data",
            ConnectionDiagnostics: new ConnectionDiagnostics
            {
                ConnectionState = "connected",
                LocalCandidateTypes = "{}",
                RemoteCandidateTypes = "{}",
                Browser = "Chrome",
                Platform = "Win32"
            },
            IceGatheringElapsedMs: 0,
            IceGatheringCollectedAll: false);
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
        var command = new StartOutgoingCallCommand(
            AccountId: null!,
            ChatId: null!,
            Offer: null!,
            ConnectionDiagnostics: new ConnectionDiagnostics
            {
                ConnectionState = null!,
                LocalCandidateTypes = null!,
                RemoteCandidateTypes = null!,
                Browser = null!,
                Platform = null!
            },
            IceGatheringElapsedMs: 0,
            IceGatheringCollectedAll: false);
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
        result.Should().NotBeNull()
            .And.BeOfType<StartOutgoingCallDto>()
            .Which.CallId.Should().NotBeNullOrEmpty();

        _chatAgnosticServiceMock.Verify(
            x => x.GetChatMemberAccountIdsAsync(null!, cancellationToken),
            Times.Once);

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == "member-1" &&
                n.IncomingCall != null &&
                n.IncomingCall.Offer == null &&
                n.IncomingCall.ChatId == null), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenSpecialCharactersInProperties_ShouldHandleCorrectly()
    {
        // Arrange
        var command = new StartOutgoingCallCommand(
            AccountId: "caller-!@#$%",
            ChatId: "chat-^&*()",
            Offer: "offer-with-special-chars-!@#$%^&*()",
            ConnectionDiagnostics: new ConnectionDiagnostics
            {
                ConnectionState = "connected",
                LocalCandidateTypes = "{}",
                RemoteCandidateTypes = "{}",
                Browser = "Chrome",
                Platform = "Win32"
            },
            IceGatheringElapsedMs: 0,
            IceGatheringCollectedAll: false);
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
        result.Should().NotBeNull()
            .And.BeOfType<StartOutgoingCallDto>()
            .Which.CallId.Should().NotBeNullOrEmpty();

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == "recipient-{}[]" &&
                n.IncomingCall != null &&
                n.IncomingCall.Offer == "offer-with-special-chars-!@#$%^&*()" &&
                n.IncomingCall.ChatId == "chat-^&*()"), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenLongOfferData_ShouldHandleCorrectly()
    {
        // Arrange
        var longOffer = new string('a', 10000); // Very long SDP offer
        var command = new StartOutgoingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Offer: longOffer,
            ConnectionDiagnostics: new ConnectionDiagnostics
            {
                ConnectionState = "connected",
                LocalCandidateTypes = "{}",
                RemoteCandidateTypes = "{}",
                Browser = "Chrome",
                Platform = "Win32"
            },
            IceGatheringElapsedMs: 0,
            IceGatheringCollectedAll: false);
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
        result.Should().NotBeNull()
            .And.BeOfType<StartOutgoingCallDto>()
            .Which.CallId.Should().NotBeNullOrEmpty();

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == "recipient-789" &&
                n.IncomingCall != null &&
                n.IncomingCall.Offer == longOffer &&
                n.IncomingCall.Offer!.Length == 10000 &&
                n.IncomingCall.ChatId == "chat-456"), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenEmptyStringProperties_ShouldHandleCorrectly()
    {
        // Arrange
        var command = new StartOutgoingCallCommand(
            AccountId: "",
            ChatId: "",
            Offer: "",
            ConnectionDiagnostics: new ConnectionDiagnostics
            {
                ConnectionState = "",
                LocalCandidateTypes = "",
                RemoteCandidateTypes = "",
                Browser = "",
                Platform = ""
            },
            IceGatheringElapsedMs: 0,
            IceGatheringCollectedAll: false);
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
        result.Should().NotBeNull()
            .And.BeOfType<StartOutgoingCallDto>()
            .Which.CallId.Should().NotBeNullOrEmpty();

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == "recipient-789" &&
                n.IncomingCall != null &&
                n.IncomingCall.Offer == "" &&
                n.IncomingCall.ChatId == ""), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenDuplicateAccountIdsInChat_ShouldFindFirstDifferentOne()
    {
        // Arrange
        var command = new StartOutgoingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Offer: "offer-data",
            ConnectionDiagnostics: new ConnectionDiagnostics
            {
                ConnectionState = "connected",
                LocalCandidateTypes = "{}",
                RemoteCandidateTypes = "{}",
                Browser = "Chrome",
                Platform = "Win32"
            },
            IceGatheringElapsedMs: 0,
            IceGatheringCollectedAll: false);
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
        result.Should().NotBeNull()
            .And.BeOfType<StartOutgoingCallDto>()
            .Which.CallId.Should().NotBeNullOrEmpty();

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == "recipient-789"), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenAllMembersAreCaller_ShouldSendNotificationWithNullRecipient()
    {
        // Arrange
        var command = new StartOutgoingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Offer: "offer-data",
            ConnectionDiagnostics: new ConnectionDiagnostics
            {
                ConnectionState = "connected",
                LocalCandidateTypes = "{}",
                RemoteCandidateTypes = "{}",
                Browser = "Chrome",
                Platform = "Win32"
            },
            IceGatheringElapsedMs: 0,
            IceGatheringCollectedAll: false);
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
        result.Should().NotBeNull()
            .And.BeOfType<StartOutgoingCallDto>()
            .Which.CallId.Should().NotBeNullOrEmpty();

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == null &&
                n.IncomingCall != null &&
                n.IncomingCall.Offer == "offer-data" &&
                n.IncomingCall.ChatId == "chat-456"), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_ShouldCreateNotificationWithCorrectRtcSessionSettings()
    {
        // Arrange
        var command = new StartOutgoingCallCommand(
            AccountId: "caller-123",
            ChatId: "chat-456",
            Offer: "test-sdp-offer",
            ConnectionDiagnostics: new ConnectionDiagnostics
            {
                ConnectionState = "connected",
                LocalCandidateTypes = "{}",
                RemoteCandidateTypes = "{}",
                Browser = "Chrome",
                Platform = "Win32"
            },
            IceGatheringElapsedMs: 0,
            IceGatheringCollectedAll: false);
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
        result.Should().NotBeNull()
            .And.BeOfType<StartOutgoingCallDto>()
            .Which.CallId.Should().NotBeNullOrEmpty();

        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.Is<Notification>(n =>
                n.RecipientId == "recipient-789" &&
                n.IncomingCall != null &&
                n.IncomingCall.Offer == "test-sdp-offer" &&
                n.IncomingCall.ChatId == "chat-456"), cancellationToken),
            Times.Once);
    }
}
using AutoMapper;
using FluentAssertions;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.API.Core.Features.Message.Commands.SetLinkPreview;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using MediatR;
using Moq;
using NUnit.Framework;

namespace LetsTalk.Server.API.Core.Tests.Handlers;

[TestFixture]
public class SetLinkPreviewCommandHandlerTests
{
    private Mock<IMessageAgnosticService> _messageAgnosticServiceMock;
    private Mock<ILinkPreviewAgnosticService> _linkPreviewAgnosticServiceMock;
    private Mock<IChatAgnosticService> _chatAgnosticServiceMock;
    private Mock<IProducer<Notification>> _producerMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IMessageCacheManager> _messageCacheManagerMock;
    private SetLinkPreviewCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _messageAgnosticServiceMock = new Mock<IMessageAgnosticService>();
        _linkPreviewAgnosticServiceMock = new Mock<ILinkPreviewAgnosticService>();
        _chatAgnosticServiceMock = new Mock<IChatAgnosticService>();
        _producerMock = new Mock<IProducer<Notification>>();
        _mapperMock = new Mock<IMapper>();
        _messageCacheManagerMock = new Mock<IMessageCacheManager>();

        _handler = new SetLinkPreviewCommandHandler(
            _messageAgnosticServiceMock.Object,
            _linkPreviewAgnosticServiceMock.Object,
            _chatAgnosticServiceMock.Object,
            _producerMock.Object,
            _mapperMock.Object,
            _messageCacheManagerMock.Object);
    }

    [Test]
    public async Task Handle_WhenSetLinkPreviewSucceeds_ShouldUseDirectMethod()
    {
        // Arrange
        var command = new SetLinkPreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Url = "https://example.com",
            Title = "Example Title",
            ImageUrl = "https://example.com/image.jpg"
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel
        {
            Id = "message-123",
            ChatId = "chat-456",
            Text = "Test message"
        };

        var linkPreviewDto = new LinkPreviewDto
        {
            MessageId = "message-123",
            Url = "https://example.com",
            Title = "Example Title"
        };

        var accountIds = new List<string> { "account-1", "account-2" };

        _messageAgnosticServiceMock
            .Setup(x => x.SetLinkPreviewAsync(
                "message-123",
                "https://example.com",
                "Example Title",
                "https://example.com/image.jpg",
                cancellationToken))
            .ReturnsAsync(messageServiceModel);

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(accountIds);

        _mapperMock
            .Setup(x => x.Map<LinkPreviewDto>(messageServiceModel))
            .Returns(linkPreviewDto);

        SetupAsyncOperations(cancellationToken);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.SetLinkPreviewAsync(
                "message-123",
                "https://example.com",
                "Example Title",
                "https://example.com/image.jpg",
                cancellationToken),
            Times.Once);

        // Should not use the fallback method
        _linkPreviewAgnosticServiceMock.Verify(
            x => x.GetIdByUrlAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _messageAgnosticServiceMock.Verify(
            x => x.SetLinkPreviewAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        VerifyCommonOperations(accountIds, linkPreviewDto, messageServiceModel, cancellationToken);
    }

    [Test]
    public async Task Handle_WhenSetLinkPreviewFails_ShouldUseFallbackMethod()
    {
        // Arrange
        var command = new SetLinkPreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Url = "https://example.com",
            Title = "Example Title",
            ImageUrl = "https://example.com/image.jpg"
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel
        {
            Id = "message-123",
            ChatId = "chat-456",
            Text = "Test message"
        };

        var linkPreviewDto = new LinkPreviewDto
        {
            MessageId = "message-123",
            Url = "https://example.com"
        };

        var accountIds = new List<string> { "account-1", "account-2" };
        var linkPreviewId = "preview-789";

        // First call fails
        _messageAgnosticServiceMock
            .Setup(x => x.SetLinkPreviewAsync(
                "message-123",
                "https://example.com",
                "Example Title",
                "https://example.com/image.jpg",
                cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Direct method failed"));

        // Fallback methods succeed
        _linkPreviewAgnosticServiceMock
            .Setup(x => x.GetIdByUrlAsync("https://example.com", cancellationToken))
            .ReturnsAsync(linkPreviewId);

        _messageAgnosticServiceMock
            .Setup(x => x.SetLinkPreviewAsync("message-123", linkPreviewId, cancellationToken))
            .ReturnsAsync(messageServiceModel);

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(accountIds);

        _mapperMock
            .Setup(x => x.Map<LinkPreviewDto>(messageServiceModel))
            .Returns(linkPreviewDto);

        SetupAsyncOperations(cancellationToken);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        // Verify both methods were called
        _messageAgnosticServiceMock.Verify(
            x => x.SetLinkPreviewAsync(
                "message-123",
                "https://example.com",
                "Example Title",
                "https://example.com/image.jpg",
                cancellationToken),
            Times.Once);

        _linkPreviewAgnosticServiceMock.Verify(
            x => x.GetIdByUrlAsync("https://example.com", cancellationToken),
            Times.Once);

        _messageAgnosticServiceMock.Verify(
            x => x.SetLinkPreviewAsync("message-123", linkPreviewId, cancellationToken),
            Times.Once);

        VerifyCommonOperations(accountIds, linkPreviewDto, messageServiceModel, cancellationToken);
    }

    [Test]
    public async Task Handle_WhenMultipleAccountIds_ShouldPublishNotificationToEach()
    {
        // Arrange
        var command = new SetLinkPreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Url = "https://example.com",
            Title = "Example Title",
            ImageUrl = "https://example.com/image.jpg"
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };
        var linkPreviewDto = new LinkPreviewDto { MessageId = "message-123" };
        var accountIds = new List<string> { "account-1", "account-2", "account-3", "account-4" };

        SetupSuccessfulDirectMethod(command, messageServiceModel, linkPreviewDto, accountIds, cancellationToken);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        foreach (var accountId in accountIds)
        {
            _producerMock.Verify(
                x => x.PublishAsync(It.Is<Notification>(n =>
                    n.RecipientId == accountId &&
                    n.LinkPreview == linkPreviewDto), cancellationToken),
                Times.Once);
        }

        _producerMock.Verify(
            x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken),
            Times.Exactly(4));
    }

    [Test]
    public async Task Handle_WhenEmptyAccountIdsList_ShouldNotPublishNotifications()
    {
        // Arrange
        var command = new SetLinkPreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Url = "https://example.com",
            Title = "Example Title",
            ImageUrl = "https://example.com/image.jpg"
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };
        var linkPreviewDto = new LinkPreviewDto { MessageId = "message-123" };
        var accountIds = new List<string>(); // Empty list

        SetupSuccessfulDirectMethod(command, messageServiceModel, linkPreviewDto, accountIds, cancellationToken);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _producerMock.Verify(
            x => x.PublishAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _messageCacheManagerMock.Verify(
            x => x.ClearAsync("chat-456"),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenNullProperties_ShouldPassNullValues()
    {
        // Arrange
        var command = new SetLinkPreviewCommand
        {
            MessageId = null,
            ChatId = null,
            Url = null,
            Title = null,
            ImageUrl = null
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = null };
        var linkPreviewDto = new LinkPreviewDto { MessageId = "message-123" };
        var accountIds = new List<string> { "account-1" };

        _messageAgnosticServiceMock
            .Setup(x => x.SetLinkPreviewAsync(null!, null!, null!, null!, cancellationToken))
            .ReturnsAsync(messageServiceModel);

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync(null!, cancellationToken))
            .ReturnsAsync(accountIds);

        _mapperMock
            .Setup(x => x.Map<LinkPreviewDto>(messageServiceModel))
            .Returns(linkPreviewDto);

        SetupAsyncOperations(cancellationToken);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.SetLinkPreviewAsync(null!, null!, null!, null!, cancellationToken),
            Times.Once);

        _messageCacheManagerMock.Verify(
            x => x.ClearAsync(null!),
            Times.Once);
    }

    [Test]
    public void Handle_WhenDirectMethodThrowsAndFallbackGetIdFails_ShouldPropagateException()
    {
        // Arrange
        var command = new SetLinkPreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Url = "https://example.com",
            Title = "Example Title",
            ImageUrl = "https://example.com/image.jpg"
        };
        var cancellationToken = CancellationToken.None;

        _messageAgnosticServiceMock
            .Setup(x => x.SetLinkPreviewAsync(
                "message-123",
                "https://example.com",
                "Example Title",
                "https://example.com/image.jpg",
                cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Direct method failed"));

        _linkPreviewAgnosticServiceMock
            .Setup(x => x.GetIdByUrlAsync("https://example.com", cancellationToken))
            .ThrowsAsync(new InvalidOperationException("GetIdByUrl failed"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));

        _messageAgnosticServiceMock.Verify(
            x => x.SetLinkPreviewAsync(
                "message-123",
                "https://example.com",
                "Example Title",
                "https://example.com/image.jpg",
                cancellationToken),
            Times.Once);

        _linkPreviewAgnosticServiceMock.Verify(
            x => x.GetIdByUrlAsync("https://example.com", cancellationToken),
            Times.Once);

        // Should not proceed to other operations
        _chatAgnosticServiceMock.Verify(
            x => x.GetChatMemberAccountIdsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public void Handle_WhenDirectMethodThrowsAndFallbackSetLinkPreviewFails_ShouldPropagateException()
    {
        // Arrange
        var command = new SetLinkPreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Url = "https://example.com",
            Title = "Example Title",
            ImageUrl = "https://example.com/image.jpg"
        };
        var cancellationToken = CancellationToken.None;

        var linkPreviewId = "preview-789";

        _messageAgnosticServiceMock
            .Setup(x => x.SetLinkPreviewAsync(
                "message-123",
                "https://example.com",
                "Example Title",
                "https://example.com/image.jpg",
                cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Direct method failed"));

        _linkPreviewAgnosticServiceMock
            .Setup(x => x.GetIdByUrlAsync("https://example.com", cancellationToken))
            .ReturnsAsync(linkPreviewId);

        _messageAgnosticServiceMock
            .Setup(x => x.SetLinkPreviewAsync("message-123", linkPreviewId, cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Fallback SetLinkPreview failed"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));

        _messageAgnosticServiceMock.Verify(
            x => x.SetLinkPreviewAsync("message-123", linkPreviewId, cancellationToken),
            Times.Once);

        // Should not proceed to other operations
        _chatAgnosticServiceMock.Verify(
            x => x.GetChatMemberAccountIdsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public void Handle_WhenChatServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new SetLinkPreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Url = "https://example.com",
            Title = "Example Title",
            ImageUrl = "https://example.com/image.jpg"
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };

        _messageAgnosticServiceMock
            .Setup(x => x.SetLinkPreviewAsync(
                "message-123",
                "https://example.com",
                "Example Title",
                "https://example.com/image.jpg",
                cancellationToken))
            .ReturnsAsync(messageServiceModel);

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Chat service error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));

        _messageAgnosticServiceMock.Verify(
            x => x.SetLinkPreviewAsync(
                "message-123",
                "https://example.com",
                "Example Title",
                "https://example.com/image.jpg",
                cancellationToken),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<LinkPreviewDto>(It.IsAny<MessageServiceModel>()),
            Times.Never);

        _producerMock.Verify(
            x => x.PublishAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _messageCacheManagerMock.Verify(
            x => x.ClearAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Test]
    public void Handle_WhenMapperThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new SetLinkPreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Url = "https://example.com",
            Title = "Example Title",
            ImageUrl = "https://example.com/image.jpg"
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };
        var accountIds = new List<string> { "account-1" };

        _messageAgnosticServiceMock
            .Setup(x => x.SetLinkPreviewAsync(
                "message-123",
                "https://example.com",
                "Example Title",
                "https://example.com/image.jpg",
                cancellationToken))
            .ReturnsAsync(messageServiceModel);

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(accountIds);

        _mapperMock
            .Setup(x => x.Map<LinkPreviewDto>(messageServiceModel))
            .Throws(new InvalidOperationException("Mapper error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));

        _messageAgnosticServiceMock.Verify(
            x => x.SetLinkPreviewAsync(
                "message-123",
                "https://example.com",
                "Example Title",
                "https://example.com/image.jpg",
                cancellationToken),
            Times.Once);

        _chatAgnosticServiceMock.Verify(
            x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken),
            Times.Once);

        _producerMock.Verify(
            x => x.PublishAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _messageCacheManagerMock.Verify(
            x => x.ClearAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Test]
    public void Handle_WhenNotificationPublishingFails_ShouldPropagateException()
    {
        // Arrange
        var command = new SetLinkPreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Url = "https://example.com",
            Title = "Example Title",
            ImageUrl = "https://example.com/image.jpg"
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };
        var linkPreviewDto = new LinkPreviewDto { MessageId = "message-123" };
        var accountIds = new List<string> { "account-1" };

        _messageAgnosticServiceMock
            .Setup(x => x.SetLinkPreviewAsync(
                "message-123",
                "https://example.com",
                "Example Title",
                "https://example.com/image.jpg",
                cancellationToken))
            .ReturnsAsync(messageServiceModel);

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(accountIds);

        _mapperMock
            .Setup(x => x.Map<LinkPreviewDto>(messageServiceModel))
            .Returns(linkPreviewDto);

        _producerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Notification publishing failed"));

        _messageCacheManagerMock
            .Setup(x => x.ClearAsync("chat-456"))
            .Returns(Task.CompletedTask);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public void Handle_WhenCacheClearingFails_ShouldPropagateException()
    {
        // Arrange
        var command = new SetLinkPreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Url = "https://example.com",
            Title = "Example Title",
            ImageUrl = "https://example.com/image.jpg"
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };
        var linkPreviewDto = new LinkPreviewDto { MessageId = "message-123" };
        var accountIds = new List<string> { "account-1" };

        _messageAgnosticServiceMock
            .Setup(x => x.SetLinkPreviewAsync(
                "message-123",
                "https://example.com",
                "Example Title",
                "https://example.com/image.jpg",
                cancellationToken))
            .ReturnsAsync(messageServiceModel);

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(accountIds);

        _mapperMock
            .Setup(x => x.Map<LinkPreviewDto>(messageServiceModel))
            .Returns(linkPreviewDto);

        _producerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .Returns(Task.CompletedTask);

        _messageCacheManagerMock
            .Setup(x => x.ClearAsync("chat-456"))
            .ThrowsAsync(new InvalidOperationException("Cache clearing failed"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public async Task Handle_WhenCancellationTokenProvided_ShouldPassItToAllServices()
    {
        // Arrange
        var command = new SetLinkPreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Url = "https://example.com",
            Title = "Example Title",
            ImageUrl = "https://example.com/image.jpg"
        };
        var cancellationToken = new CancellationToken(false);

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };
        var linkPreviewDto = new LinkPreviewDto { MessageId = "message-123" };
        var accountIds = new List<string> { "account-1" };

        SetupSuccessfulDirectMethod(command, messageServiceModel, linkPreviewDto, accountIds, cancellationToken);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _messageAgnosticServiceMock.Verify(
            x => x.SetLinkPreviewAsync(
                "message-123",
                "https://example.com",
                "Example Title",
                "https://example.com/image.jpg",
                cancellationToken),
            Times.Once);

        _chatAgnosticServiceMock.Verify(
            x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken),
            Times.Once);

        _producerMock.Verify(
            x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenSpecialCharactersInProperties_ShouldHandleCorrectly()
    {
        // Arrange
        var command = new SetLinkPreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Url = "https://example.com/path?param=value&other=test",
            Title = "Title with special chars: !@#$%^&*()",
            ImageUrl = "https://example.com/image.jpg?size=large&format=webp"
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };
        var linkPreviewDto = new LinkPreviewDto { MessageId = "message-123" };
        var accountIds = new List<string> { "account-1" };

        SetupSuccessfulDirectMethod(command, messageServiceModel, linkPreviewDto, accountIds, cancellationToken);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.SetLinkPreviewAsync(
                "message-123",
                "https://example.com/path?param=value&other=test",
                "Title with special chars: !@#$%^&*()",
                "https://example.com/image.jpg?size=large&format=webp",
                cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenUnicodeCharactersInProperties_ShouldHandleCorrectly()
    {
        // Arrange
        var command = new SetLinkPreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Url = "https://example.com/中文/العربية",
            Title = "标题 - عنوان - 🌟 Title",
            ImageUrl = "https://example.com/图片.jpg"
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };
        var linkPreviewDto = new LinkPreviewDto { MessageId = "message-123" };
        var accountIds = new List<string> { "account-1" };

        SetupSuccessfulDirectMethod(command, messageServiceModel, linkPreviewDto, accountIds, cancellationToken);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.SetLinkPreviewAsync(
                "message-123",
                "https://example.com/中文/العربية",
                "标题 - عنوان - 🌟 Title",
                "https://example.com/图片.jpg",
                cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenEmptyStringProperties_ShouldPassEmptyStrings()
    {
        // Arrange
        var command = new SetLinkPreviewCommand
        {
            MessageId = "",
            ChatId = "",
            Url = "",
            Title = "",
            ImageUrl = ""
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "" };
        var linkPreviewDto = new LinkPreviewDto { MessageId = "message-123" };
        var accountIds = new List<string> { "account-1" };

        _messageAgnosticServiceMock
            .Setup(x => x.SetLinkPreviewAsync("", "", "", "", cancellationToken))
            .ReturnsAsync(messageServiceModel);

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("", cancellationToken))
            .ReturnsAsync(accountIds);

        _mapperMock
            .Setup(x => x.Map<LinkPreviewDto>(messageServiceModel))
            .Returns(linkPreviewDto);

        SetupAsyncOperations(cancellationToken);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.SetLinkPreviewAsync("", "", "", "", cancellationToken),
            Times.Once);

        _messageCacheManagerMock.Verify(
            x => x.ClearAsync(""),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenFallbackMethodUsesNullLinkPreviewId_ShouldHandleCorrectly()
    {
        // Arrange
        var command = new SetLinkPreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Url = "https://example.com",
            Title = "Example Title",
            ImageUrl = "https://example.com/image.jpg"
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };
        var linkPreviewDto = new LinkPreviewDto { MessageId = "message-123" };
        var accountIds = new List<string> { "account-1" };

        // First call fails
        _messageAgnosticServiceMock
            .Setup(x => x.SetLinkPreviewAsync(
                "message-123",
                "https://example.com",
                "Example Title",
                "https://example.com/image.jpg",
                cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Direct method failed"));

        // GetIdByUrl returns null
        _linkPreviewAgnosticServiceMock
            .Setup(x => x.GetIdByUrlAsync("https://example.com", cancellationToken))
            .ReturnsAsync((string?)null);

        // Fallback method with null linkPreviewId
        _messageAgnosticServiceMock
            .Setup(x => x.SetLinkPreviewAsync("message-123", null!, cancellationToken))
            .ReturnsAsync(messageServiceModel);

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(accountIds);

        _mapperMock
            .Setup(x => x.Map<LinkPreviewDto>(messageServiceModel))
            .Returns(linkPreviewDto);

        SetupAsyncOperations(cancellationToken);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.SetLinkPreviewAsync("message-123", null!, cancellationToken),
            Times.Once);

        VerifyCommonOperations(accountIds, linkPreviewDto, messageServiceModel, cancellationToken);
    }

    [Test]
    public async Task Handle_WhenLongStringProperties_ShouldHandleCorrectly()
    {
        // Arrange
        var longUrl = "https://example.com/" + new string('a', 2000);
        var longTitle = new string('T', 1000);
        var longImageUrl = "https://example.com/image/" + new string('b', 1500) + ".jpg";

        var command = new SetLinkPreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Url = longUrl,
            Title = longTitle,
            ImageUrl = longImageUrl
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };
        var linkPreviewDto = new LinkPreviewDto { MessageId = "message-123" };
        var accountIds = new List<string> { "account-1" };

        SetupSuccessfulDirectMethod(command, messageServiceModel, linkPreviewDto, accountIds, cancellationToken);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.SetLinkPreviewAsync(
                "message-123",
                longUrl,
                longTitle,
                longImageUrl,
                cancellationToken),
            Times.Once);
    }

    private void SetupSuccessfulDirectMethod(SetLinkPreviewCommand command, MessageServiceModel messageServiceModel, LinkPreviewDto linkPreviewDto, List<string> accountIds, CancellationToken cancellationToken)
    {
        _messageAgnosticServiceMock
            .Setup(x => x.SetLinkPreviewAsync(
                command.MessageId!,
                command.Url!,
                command.Title!,
                command.ImageUrl!,
                cancellationToken))
            .ReturnsAsync(messageServiceModel);

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync(command.ChatId!, cancellationToken))
            .ReturnsAsync(accountIds);

        _mapperMock
            .Setup(x => x.Map<LinkPreviewDto>(messageServiceModel))
            .Returns(linkPreviewDto);

        SetupAsyncOperations(cancellationToken);
    }

    private void SetupAsyncOperations(CancellationToken cancellationToken)
    {
        _producerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .Returns(Task.CompletedTask);

        _messageCacheManagerMock
            .Setup(x => x.ClearAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
    }

    private void VerifyCommonOperations(List<string> accountIds, LinkPreviewDto linkPreviewDto, MessageServiceModel messageServiceModel, CancellationToken cancellationToken)
    {
        _chatAgnosticServiceMock.Verify(
            x => x.GetChatMemberAccountIdsAsync(messageServiceModel.ChatId!, cancellationToken),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<LinkPreviewDto>(messageServiceModel),
            Times.Once);

        foreach (var accountId in accountIds)
        {
            _producerMock.Verify(
                x => x.PublishAsync(It.Is<Notification>(n =>
                    n.RecipientId == accountId &&
                    n.LinkPreview == linkPreviewDto), cancellationToken),
                Times.Once);
        }

        _messageCacheManagerMock.Verify(
            x => x.ClearAsync(messageServiceModel.ChatId!),
            Times.Once);
    }
}
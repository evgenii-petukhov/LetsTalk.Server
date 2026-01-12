using AutoMapper;
using FluentAssertions;
using FluentValidation.Results;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.API.Core.Features.Message.Commands.CreateMessage;
using LetsTalk.Server.API.Core.Models.HtmlGenerator;
using LetsTalk.Server.API.Models.Message;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.Enums;
using Moq;
using NUnit.Framework;

namespace LetsTalk.Server.API.Core.Tests.Handlers;

[TestFixture]
public class CreateMessageCommandHandlerTests
{
    private Mock<IChatAgnosticService> _chatAgnosticServiceMock;
    private Mock<IHtmlGenerator> _htmlGeneratorMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IMessageCacheManager> _messageCacheManagerMock;
    private Mock<IMessageAgnosticService> _messageAgnosticServiceMock;
    private Mock<ILinkPreviewAgnosticService> _linkPreviewAgnosticServiceMock;
    private Mock<IChatCacheManager> _chatCacheManagerMock;
    private Mock<IProducer<Notification>> _notificationProducerMock;
    private Mock<IProducer<LinkPreviewRequest>> _linkPreviewProducerMock;
    private Mock<IProducer<ImageResizeRequest>> _imageResizeProducerMock;
    private CreateMessageCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _chatAgnosticServiceMock = new Mock<IChatAgnosticService>();
        _htmlGeneratorMock = new Mock<IHtmlGenerator>();
        _mapperMock = new Mock<IMapper>();
        _messageCacheManagerMock = new Mock<IMessageCacheManager>();
        _messageAgnosticServiceMock = new Mock<IMessageAgnosticService>();
        _linkPreviewAgnosticServiceMock = new Mock<ILinkPreviewAgnosticService>();
        _chatCacheManagerMock = new Mock<IChatCacheManager>();
        _notificationProducerMock = new Mock<IProducer<Notification>>();
        _linkPreviewProducerMock = new Mock<IProducer<LinkPreviewRequest>>();
        _imageResizeProducerMock = new Mock<IProducer<ImageResizeRequest>>();

        _handler = new CreateMessageCommandHandler(
            _chatAgnosticServiceMock.Object,
            _htmlGeneratorMock.Object,
            _mapperMock.Object,
            _messageCacheManagerMock.Object,
            _messageAgnosticServiceMock.Object,
            _linkPreviewAgnosticServiceMock.Object,
            _chatCacheManagerMock.Object,
            _notificationProducerMock.Object,
            _linkPreviewProducerMock.Object,
            _imageResizeProducerMock.Object);
    }

    [Test]
    public async Task Handle_WhenValidTextMessage_ShouldCreateMessageSuccessfully()
    {
        // Arrange
        var command = new CreateMessageCommand
        {
            SenderId = "sender-123",
            ChatId = "chat-456",
            Text = "Hello world",
            Image = null
        };
        var cancellationToken = CancellationToken.None;

        SetupValidValidation();
        
        var html = "<p>Hello world</p>";
        var url = "https://example.com";
        _htmlGeneratorMock
            .Setup(x => x.GetHtml("Hello world"))
            .Returns(new HtmlGeneratorResult(html, url));

        var linkPreviewId = "preview-123";
        _linkPreviewAgnosticServiceMock
            .Setup(x => x.GetIdByUrlAsync(url, cancellationToken))
            .ReturnsAsync(linkPreviewId);

        var messageDto = new MessageDto { Id = "msg-789", Text = "Hello world" };
        var message = new MessageServiceModel { Id = "msg-789", Text = "Hello world" };
        
        _messageAgnosticServiceMock
            .Setup(x => x.CreateMessageAsync("sender-123", "chat-456", "Hello world", html, linkPreviewId, cancellationToken))
            .ReturnsAsync(message);

        _mapperMock
            .Setup(x => x.Map<MessageDto>(message))
            .Returns(messageDto);

        var accountIds = new List<string> { "sender-123", "recipient-456" };
        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(accountIds);

        SetupAsyncOperations(cancellationToken);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Dto.Should().Be(messageDto);
        result.Url.Should().Be(url);

        VerifyMessageCreationWorkflow(command, html, linkPreviewId, cancellationToken);
        VerifyNotificationPublishing(accountIds, messageDto, cancellationToken);
        VerifyCacheClearingOperations("chat-456", accountIds);
    }

    [Test]
    public async Task Handle_WhenValidMessageWithImage_ShouldCreateMessageWithImageSuccessfully()
    {
        // Arrange
        var imageRequest = new ImageRequestModel
        {
            Id = "image-123",
            Width = 800,
            Height = 600,
            ImageFormat = (int)ImageFormats.Jpeg,
            FileStorageTypeId = (int)FileStorageTypes.Local
        };

        var command = new CreateMessageCommand
        {
            SenderId = "sender-123",
            ChatId = "chat-456",
            Text = "Check this image",
            Image = imageRequest
        };
        var cancellationToken = CancellationToken.None;

        SetupValidValidation();
        
        var html = "<p>Check this image</p>";
        var url = "";
        _htmlGeneratorMock
            .Setup(x => x.GetHtml("Check this image"))
            .Returns(new HtmlGeneratorResult(html, url));

        var messageDto = new MessageDto { Id = "msg-789", Text = "Check this image" };
        var message = new MessageServiceModel { Id = "msg-789", Text = "Check this image" };
        
        _messageAgnosticServiceMock
            .Setup(x => x.CreateMessageAsync(
                "sender-123", 
                "chat-456", 
                "Check this image", 
                html, 
                "image-123", 
                800, 
                600, 
                ImageFormats.Jpeg, 
                FileStorageTypes.Local, 
                cancellationToken))
            .ReturnsAsync(message);

        _mapperMock
            .Setup(x => x.Map<MessageDto>(message))
            .Returns(messageDto);

        var accountIds = new List<string> { "sender-123", "recipient-456" };
        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(accountIds);

        SetupAsyncOperations(cancellationToken);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Dto.Should().Be(messageDto);
        result.Url.Should().Be(url);

        _messageAgnosticServiceMock.Verify(
            x => x.CreateMessageAsync(
                "sender-123", 
                "chat-456", 
                "Check this image", 
                html, 
                "image-123", 
                800, 
                600, 
                ImageFormats.Jpeg, 
                FileStorageTypes.Local, 
                cancellationToken),
            Times.Once);

        VerifyImageResizePublishing(accountIds, messageDto, imageRequest, cancellationToken);
    }

    [Test]
    public async Task Handle_WhenTextWithoutUrl_ShouldNotRequestLinkPreview()
    {
        // Arrange
        var command = new CreateMessageCommand
        {
            SenderId = "sender-123",
            ChatId = "chat-456",
            Text = "Simple text message",
            Image = null
        };
        var cancellationToken = CancellationToken.None;

        SetupValidValidation();
        
        var html = "<p>Simple text message</p>";
        var url = "";
        _htmlGeneratorMock
            .Setup(x => x.GetHtml("Simple text message"))
            .Returns(new HtmlGeneratorResult(html, url));

        var messageDto = new MessageDto { Id = "msg-789" };
        var message = new MessageServiceModel { Id = "msg-789" };
        
        _messageAgnosticServiceMock
            .Setup(x => x.CreateMessageAsync("sender-123", "chat-456", "Simple text message", html, null, cancellationToken))
            .ReturnsAsync(message);

        _mapperMock
            .Setup(x => x.Map<MessageDto>(message))
            .Returns(messageDto);

        var accountIds = new List<string> { "sender-123" };
        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(accountIds);

        SetupAsyncOperations(cancellationToken);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Url.Should().Be(url);

        _linkPreviewAgnosticServiceMock.Verify(
            x => x.GetIdByUrlAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _linkPreviewProducerMock.Verify(
            x => x.PublishAsync(It.IsAny<LinkPreviewRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task Handle_WhenUrlExistsButLinkPreviewAlreadyExists_ShouldNotPublishLinkPreviewRequest()
    {
        // Arrange
        var command = new CreateMessageCommand
        {
            SenderId = "sender-123",
            ChatId = "chat-456",
            Text = "Check this link",
            Image = null
        };
        var cancellationToken = CancellationToken.None;

        SetupValidValidation();
        
        var html = "<p>Check this link</p>";
        var url = "https://example.com";
        _htmlGeneratorMock
            .Setup(x => x.GetHtml("Check this link"))
            .Returns(new HtmlGeneratorResult(html, url));

        var existingLinkPreviewId = "existing-preview-123";
        _linkPreviewAgnosticServiceMock
            .Setup(x => x.GetIdByUrlAsync(url, cancellationToken))
            .ReturnsAsync(existingLinkPreviewId);

        var messageDto = new MessageDto { Id = "msg-789" };
        var message = new MessageServiceModel { Id = "msg-789" };
        
        _messageAgnosticServiceMock
            .Setup(x => x.CreateMessageAsync("sender-123", "chat-456", "Check this link", html, existingLinkPreviewId, cancellationToken))
            .ReturnsAsync(message);

        _mapperMock
            .Setup(x => x.Map<MessageDto>(message))
            .Returns(messageDto);

        var accountIds = new List<string> { "sender-123" };
        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(accountIds);

        SetupAsyncOperations(cancellationToken);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _linkPreviewProducerMock.Verify(
            x => x.PublishAsync(It.IsAny<LinkPreviewRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task Handle_WhenUrlExistsAndNoLinkPreview_ShouldPublishLinkPreviewRequest()
    {
        // Arrange
        var command = new CreateMessageCommand
        {
            SenderId = "sender-123",
            ChatId = "chat-456",
            Text = "New link to preview",
            Image = null
        };
        var cancellationToken = CancellationToken.None;

        SetupValidValidation();
        
        var html = "<p>New link to preview</p>";
        var url = "https://newsite.com";
        _htmlGeneratorMock
            .Setup(x => x.GetHtml("New link to preview"))
            .Returns(new HtmlGeneratorResult(html, url));

        _linkPreviewAgnosticServiceMock
            .Setup(x => x.GetIdByUrlAsync(url, cancellationToken))
            .ReturnsAsync((string?)null);

        var messageDto = new MessageDto { Id = "msg-789" };
        var message = new MessageServiceModel { Id = "msg-789" };
        
        _messageAgnosticServiceMock
            .Setup(x => x.CreateMessageAsync("sender-123", "chat-456", "New link to preview", html, null, cancellationToken))
            .ReturnsAsync(message);

        _mapperMock
            .Setup(x => x.Map<MessageDto>(message))
            .Returns(messageDto);

        var accountIds = new List<string> { "sender-123", "recipient-456" };
        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(accountIds);

        SetupAsyncOperations(cancellationToken);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _linkPreviewProducerMock.Verify(
            x => x.PublishAsync(It.Is<LinkPreviewRequest>(req =>
                req.AccountIds == accountIds &&
                req.MessageId == messageDto.Id &&
                req.Url == url &&
                req.ChatId == "chat-456"), cancellationToken),
            Times.Once);
    }

    [Test]
    public void Handle_WhenValidationFails_ShouldThrowBadRequestException()
    {
        // Arrange
        var command = new CreateMessageCommand
        {
            SenderId = "sender-123",
            ChatId = "invalid-chat",
            Text = "Hello world"
        };
        var cancellationToken = CancellationToken.None;

        var validationFailures = new List<ValidationFailure>
        {
            new("ChatId", "Chat must exist")
        };
        var validationResult = new ValidationResult(validationFailures);

        _chatAgnosticServiceMock
            .Setup(x => x.IsChatIdValidAsync("invalid-chat", cancellationToken))
            .ReturnsAsync(false);

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public void Handle_WhenHtmlGeneratorThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new CreateMessageCommand
        {
            SenderId = "sender-123",
            ChatId = "chat-456",
            Text = "Hello world"
        };
        var cancellationToken = CancellationToken.None;

        SetupValidValidation();

        _htmlGeneratorMock
            .Setup(x => x.GetHtml("Hello world"))
            .Throws(new InvalidOperationException("HTML generation failed"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public void Handle_WhenMessageServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new CreateMessageCommand
        {
            SenderId = "sender-123",
            ChatId = "chat-456",
            Text = "Hello world"
        };
        var cancellationToken = CancellationToken.None;

        SetupValidValidation();
        
        var html = "<p>Hello world</p>";
        var url = "";
        _htmlGeneratorMock
            .Setup(x => x.GetHtml("Hello world"))
            .Returns(new HtmlGeneratorResult(html, url));

        _messageAgnosticServiceMock
            .Setup(x => x.CreateMessageAsync("sender-123", "chat-456", "Hello world", html, null, cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Message creation failed"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public void Handle_WhenMapperThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new CreateMessageCommand
        {
            SenderId = "sender-123",
            ChatId = "chat-456",
            Text = "Hello world"
        };
        var cancellationToken = CancellationToken.None;

        SetupValidValidation();
        
        var html = "<p>Hello world</p>";
        var url = "";
        _htmlGeneratorMock
            .Setup(x => x.GetHtml("Hello world"))
            .Returns(new HtmlGeneratorResult(html, url));

        var message = new MessageServiceModel { Id = "msg-789" };
        _messageAgnosticServiceMock
            .Setup(x => x.CreateMessageAsync("sender-123", "chat-456", "Hello world", html, null, cancellationToken))
            .ReturnsAsync(message);

        _mapperMock
            .Setup(x => x.Map<MessageDto>(message))
            .Throws(new InvalidOperationException("Mapping failed"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public async Task Handle_WhenMultipleAccountIds_ShouldPublishNotificationsToAll()
    {
        // Arrange
        var command = new CreateMessageCommand
        {
            SenderId = "sender-123",
            ChatId = "chat-456",
            Text = "Group message"
        };
        var cancellationToken = CancellationToken.None;

        SetupValidValidation();
        
        var html = "<p>Group message</p>";
        var url = "";
        _htmlGeneratorMock
            .Setup(x => x.GetHtml("Group message"))
            .Returns(new HtmlGeneratorResult(html, url));

        var messageDto = new MessageDto { Id = "msg-789", Text = "Group message" };
        var message = new MessageServiceModel { Id = "msg-789", Text = "Group message" };
        
        _messageAgnosticServiceMock
            .Setup(x => x.CreateMessageAsync("sender-123", "chat-456", "Group message", html, null, cancellationToken))
            .ReturnsAsync(message);

        _mapperMock
            .Setup(x => x.Map<MessageDto>(message))
            .Returns(messageDto);

        var accountIds = new List<string> { "sender-123", "user-1", "user-2", "user-3" };
        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(accountIds);

        SetupAsyncOperations(cancellationToken);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        foreach (var accountId in accountIds)
        {
            _notificationProducerMock.Verify(
                x => x.PublishAsync(It.Is<Notification>(n =>
                    n.RecipientId == accountId &&
                    n.Message.Id == messageDto.Id &&
                    n.Message.IsMine == (accountId == "sender-123")), cancellationToken),
                Times.Once);

            _chatCacheManagerMock.Verify(
                x => x.ClearAsync(accountId),
                Times.Once);
        }
    }

    [Test]
    public void Handle_WhenNotificationPublishingFails_ShouldPropagateException()
    {
        // Arrange
        var command = new CreateMessageCommand
        {
            SenderId = "sender-123",
            ChatId = "chat-456",
            Text = "Hello world"
        };
        var cancellationToken = CancellationToken.None;

        SetupValidValidation();
        SetupBasicMessageCreation(command, cancellationToken);

        var accountIds = new List<string> { "sender-123" };
        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(accountIds);

        // Setup other operations to succeed
        _chatCacheManagerMock
            .Setup(x => x.ClearAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _linkPreviewProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<LinkPreviewRequest>(), cancellationToken))
            .Returns(Task.CompletedTask);

        _imageResizeProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<ImageResizeRequest>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Setup notification producer to fail - this must be last to override any other setups
        _notificationProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Notification publishing failed"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public void Handle_WhenCacheClearingFails_ShouldPropagateException()
    {
        // Arrange
        var command = new CreateMessageCommand
        {
            SenderId = "sender-123",
            ChatId = "chat-456",
            Text = "Hello world"
        };
        var cancellationToken = CancellationToken.None;

        SetupValidValidation();
        SetupBasicMessageCreation(command, cancellationToken);

        var accountIds = new List<string> { "sender-123" };
        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(accountIds);

        // Setup other operations to succeed
        _notificationProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .Returns(Task.CompletedTask);

        _linkPreviewProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<LinkPreviewRequest>(), cancellationToken))
            .Returns(Task.CompletedTask);

        _imageResizeProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<ImageResizeRequest>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Setup message cache manager to fail - this must be last to override any other setups
        _messageCacheManagerMock
            .Setup(x => x.ClearAsync("chat-456"))
            .ThrowsAsync(new InvalidOperationException("Cache clearing failed"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public async Task Handle_WhenNullText_ShouldHandleGracefully()
    {
        // Arrange
        var command = new CreateMessageCommand
        {
            SenderId = "sender-123",
            ChatId = "chat-456",
            Text = null
        };
        var cancellationToken = CancellationToken.None;

        SetupValidValidation();
        
        _htmlGeneratorMock
            .Setup(x => x.GetHtml(null!))
            .Returns(new HtmlGeneratorResult("", ""));

        var messageDto = new MessageDto { Id = "msg-789" };
        var message = new MessageServiceModel { Id = "msg-789" };
        
        _messageAgnosticServiceMock
            .Setup(x => x.CreateMessageAsync("sender-123", "chat-456", null!, "", null, cancellationToken))
            .ReturnsAsync(message);

        _mapperMock
            .Setup(x => x.Map<MessageDto>(message))
            .Returns(messageDto);

        var accountIds = new List<string> { "sender-123" };
        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(accountIds);

        SetupAsyncOperations(cancellationToken);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Dto.Should().Be(messageDto);
    }

    [Test]
    public async Task Handle_WhenEmptyAccountIdsList_ShouldNotPublishNotifications()
    {
        // Arrange
        var command = new CreateMessageCommand
        {
            SenderId = "sender-123",
            ChatId = "chat-456",
            Text = "Hello world"
        };
        var cancellationToken = CancellationToken.None;

        SetupValidValidation();
        SetupBasicMessageCreation(command, cancellationToken);

        var accountIds = new List<string>();
        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(accountIds);

        SetupAsyncOperations(cancellationToken);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _notificationProducerMock.Verify(
            x => x.PublishAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _chatCacheManagerMock.Verify(
            x => x.ClearAsync(It.IsAny<string>()),
            Times.Never); // No account cache clearing since no accounts
    }

    private void SetupValidValidation()
    {
        _chatAgnosticServiceMock
            .Setup(x => x.IsChatIdValidAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    private void SetupBasicMessageCreation(CreateMessageCommand command, CancellationToken cancellationToken)
    {
        var html = "<p>Hello world</p>";
        var url = "";
        _htmlGeneratorMock
            .Setup(x => x.GetHtml(command.Text!))
            .Returns(new HtmlGeneratorResult(html, url));

        var messageDto = new MessageDto { Id = "msg-789" };
        var message = new MessageServiceModel { Id = "msg-789" };
        
        _messageAgnosticServiceMock
            .Setup(x => x.CreateMessageAsync(command.SenderId!, command.ChatId!, command.Text!, html, null, cancellationToken))
            .ReturnsAsync(message);

        _mapperMock
            .Setup(x => x.Map<MessageDto>(message))
            .Returns(messageDto);
    }

    private void SetupAsyncOperations(CancellationToken cancellationToken)
    {
        _messageCacheManagerMock
            .Setup(x => x.ClearAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _chatCacheManagerMock
            .Setup(x => x.ClearAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _notificationProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .Returns(Task.CompletedTask);

        _linkPreviewProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<LinkPreviewRequest>(), cancellationToken))
            .Returns(Task.CompletedTask);

        _imageResizeProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<ImageResizeRequest>(), cancellationToken))
            .Returns(Task.CompletedTask);
    }

    private void SetupOtherAsyncOperations(CancellationToken cancellationToken)
    {
        _chatCacheManagerMock
            .Setup(x => x.ClearAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _notificationProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .Returns(Task.CompletedTask);

        _linkPreviewProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<LinkPreviewRequest>(), cancellationToken))
            .Returns(Task.CompletedTask);

        _imageResizeProducerMock
            .Setup(x => x.PublishAsync(It.IsAny<ImageResizeRequest>(), cancellationToken))
            .Returns(Task.CompletedTask);
    }

    private void VerifyMessageCreationWorkflow(CreateMessageCommand command, string html, string linkPreviewId, CancellationToken cancellationToken)
    {
        _htmlGeneratorMock.Verify(x => x.GetHtml(command.Text!), Times.Once);
        _messageAgnosticServiceMock.Verify(
            x => x.CreateMessageAsync(command.SenderId!, command.ChatId!, command.Text!, html, linkPreviewId, cancellationToken),
            Times.Once);
        _messageCacheManagerMock.Verify(x => x.ClearAsync(command.ChatId!), Times.Once);
    }

    private void VerifyNotificationPublishing(List<string> accountIds, MessageDto messageDto, CancellationToken cancellationToken)
    {
        foreach (var accountId in accountIds)
        {
            _notificationProducerMock.Verify(
                x => x.PublishAsync(It.Is<Notification>(n =>
                    n.RecipientId == accountId &&
                    n.Message.Id == messageDto.Id), cancellationToken),
                Times.Once);
        }
    }

    private void VerifyCacheClearingOperations(string chatId, List<string> accountIds)
    {
        _messageCacheManagerMock.Verify(x => x.ClearAsync(chatId), Times.Once);
        
        foreach (var accountId in accountIds)
        {
            _chatCacheManagerMock.Verify(x => x.ClearAsync(accountId), Times.Once);
        }
    }

    private void VerifyImageResizePublishing(List<string> accountIds, MessageDto messageDto, ImageRequestModel imageRequest, CancellationToken cancellationToken)
    {
        _imageResizeProducerMock.Verify(
            x => x.PublishAsync(It.Is<ImageResizeRequest>(req =>
                req.AccountIds == accountIds &&
                req.MessageId == messageDto.Id &&
                req.ImageId == imageRequest.Id &&
                req.ChatId == "chat-456" &&
                req.FileStorageTypeId == imageRequest.FileStorageTypeId), cancellationToken),
            Times.Once);
    }
}
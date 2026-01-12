using AutoMapper;
using FluentAssertions;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.API.Core.Features.Message.Commands.SetImagePreview;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Kafka.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.Enums;
using MediatR;
using Moq;
using NUnit.Framework;

namespace LetsTalk.Server.API.Core.Tests.Handlers;

[TestFixture]
public class SetImagePreviewCommandHandlerTests
{
    private Mock<IMessageAgnosticService> _messageAgnosticServiceMock;
    private Mock<IChatAgnosticService> _chatAgnosticServiceMock;
    private Mock<IProducer<Notification>> _producerMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IMessageCacheManager> _messageCacheManagerMock;
    private SetImagePreviewCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _messageAgnosticServiceMock = new Mock<IMessageAgnosticService>();
        _chatAgnosticServiceMock = new Mock<IChatAgnosticService>();
        _producerMock = new Mock<IProducer<Notification>>();
        _mapperMock = new Mock<IMapper>();
        _messageCacheManagerMock = new Mock<IMessageCacheManager>();

        _handler = new SetImagePreviewCommandHandler(
            _messageAgnosticServiceMock.Object,
            _chatAgnosticServiceMock.Object,
            _producerMock.Object,
            _mapperMock.Object,
            _messageCacheManagerMock.Object);
    }

    [Test]
    public async Task Handle_WhenValidRequest_ShouldSaveImagePreviewAndPublishNotifications()
    {
        // Arrange
        var command = new SetImagePreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Filename = "image.jpg",
            ImageFormat = ImageFormats.Jpeg,
            Width = 800,
            Height = 600,
            FileStorageTypeId = FileStorageTypes.Local
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel
        {
            Id = "message-123",
            ChatId = "chat-456",
            Text = "Test message"
        };

        var imagePreviewDto = new ImagePreviewDto
        {
            MessageId = "message-123",
            Id = "preview-123"
        };

        var accountIds = new List<string> { "account-1", "account-2" };

        _messageAgnosticServiceMock
            .Setup(x => x.SaveImagePreviewAsync(
                "message-123",
                "image.jpg",
                ImageFormats.Jpeg,
                800,
                600,
                FileStorageTypes.Local,
                cancellationToken))
            .ReturnsAsync(messageServiceModel);

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(accountIds);

        _mapperMock
            .Setup(x => x.Map<ImagePreviewDto>(messageServiceModel))
            .Returns(imagePreviewDto);

        _producerMock
            .Setup(x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken))
            .Returns(Task.CompletedTask);

        _messageCacheManagerMock
            .Setup(x => x.ClearAsync("chat-456"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.SaveImagePreviewAsync(
                "message-123",
                "image.jpg",
                ImageFormats.Jpeg,
                800,
                600,
                FileStorageTypes.Local,
                cancellationToken),
            Times.Once);

        _chatAgnosticServiceMock.Verify(
            x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<ImagePreviewDto>(messageServiceModel),
            Times.Once);

        foreach (var accountId in accountIds)
        {
            _producerMock.Verify(
                x => x.PublishAsync(It.Is<Notification>(n =>
                    n.RecipientId == accountId &&
                    n.ImagePreview == imagePreviewDto), cancellationToken),
                Times.Once);
        }

        _messageCacheManagerMock.Verify(
            x => x.ClearAsync("chat-456"),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenDifferentImageFormats_ShouldHandleAllFormats()
    {
        // Arrange
        var formats = new[] { ImageFormats.Jpeg, ImageFormats.Png, ImageFormats.Gif, ImageFormats.Webp };

        foreach (var format in formats)
        {
            var command = new SetImagePreviewCommand
            {
                MessageId = "message-123",
                ChatId = "chat-456",
                Filename = $"image.{format.ToString().ToLower()}",
                ImageFormat = format,
                Width = 800,
                Height = 600,
                FileStorageTypeId = FileStorageTypes.Local
            };
            var cancellationToken = CancellationToken.None;

            var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };
            var imagePreviewDto = new ImagePreviewDto { MessageId = "message-123" };
            var accountIds = new List<string> { "account-1" };

            _messageAgnosticServiceMock
                .Setup(x => x.SaveImagePreviewAsync(
                    "message-123",
                    $"image.{format.ToString().ToLower()}",
                    format,
                    800,
                    600,
                    FileStorageTypes.Local,
                    cancellationToken))
                .ReturnsAsync(messageServiceModel);

            _chatAgnosticServiceMock
                .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
                .ReturnsAsync(accountIds);

            _mapperMock
                .Setup(x => x.Map<ImagePreviewDto>(messageServiceModel))
                .Returns(imagePreviewDto);

            SetupAsyncOperations(cancellationToken);

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.Should().Be(Unit.Value);

            _messageAgnosticServiceMock.Verify(
                x => x.SaveImagePreviewAsync(
                    "message-123",
                    $"image.{format.ToString().ToLower()}",
                    format,
                    800,
                    600,
                    FileStorageTypes.Local,
                    cancellationToken),
                Times.Once);
        }
    }

    [Test]
    public async Task Handle_WhenDifferentFileStorageTypes_ShouldHandleAllTypes()
    {
        // Arrange
        var storageTypes = new[] { FileStorageTypes.Local, FileStorageTypes.AmazonS3, FileStorageTypes.AzureBlobStorage };

        foreach (var storageType in storageTypes)
        {
            var command = new SetImagePreviewCommand
            {
                MessageId = "message-123",
                ChatId = "chat-456",
                Filename = "image.jpg",
                ImageFormat = ImageFormats.Jpeg,
                Width = 800,
                Height = 600,
                FileStorageTypeId = storageType
            };
            var cancellationToken = CancellationToken.None;

            var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };
            var imagePreviewDto = new ImagePreviewDto { MessageId = "message-123" };
            var accountIds = new List<string> { "account-1" };

            _messageAgnosticServiceMock
                .Setup(x => x.SaveImagePreviewAsync(
                    "message-123",
                    "image.jpg",
                    ImageFormats.Jpeg,
                    800,
                    600,
                    storageType,
                    cancellationToken))
                .ReturnsAsync(messageServiceModel);

            _chatAgnosticServiceMock
                .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
                .ReturnsAsync(accountIds);

            _mapperMock
                .Setup(x => x.Map<ImagePreviewDto>(messageServiceModel))
                .Returns(imagePreviewDto);

            SetupAsyncOperations(cancellationToken);

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.Should().Be(Unit.Value);

            _messageAgnosticServiceMock.Verify(
                x => x.SaveImagePreviewAsync(
                    "message-123",
                    "image.jpg",
                    ImageFormats.Jpeg,
                    800,
                    600,
                    storageType,
                    cancellationToken),
                Times.Once);
        }
    }

    [Test]
    public async Task Handle_WhenZeroDimensions_ShouldPassZeroValues()
    {
        // Arrange
        var command = new SetImagePreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Filename = "image.jpg",
            ImageFormat = ImageFormats.Jpeg,
            Width = 0,
            Height = 0,
            FileStorageTypeId = FileStorageTypes.Local
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };
        var imagePreviewDto = new ImagePreviewDto { MessageId = "message-123" };
        var accountIds = new List<string> { "account-1" };

        SetupBasicMocks(messageServiceModel, imagePreviewDto, accountIds, cancellationToken);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.SaveImagePreviewAsync(
                "message-123",
                "image.jpg",
                ImageFormats.Jpeg,
                0,
                0,
                FileStorageTypes.Local,
                cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenNegativeDimensions_ShouldPassNegativeValues()
    {
        // Arrange
        var command = new SetImagePreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Filename = "image.jpg",
            ImageFormat = ImageFormats.Jpeg,
            Width = -100,
            Height = -200,
            FileStorageTypeId = FileStorageTypes.Local
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };
        var imagePreviewDto = new ImagePreviewDto { MessageId = "message-123" };
        var accountIds = new List<string> { "account-1" };

        SetupBasicMocks(messageServiceModel, imagePreviewDto, accountIds, cancellationToken);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.SaveImagePreviewAsync(
                "message-123",
                "image.jpg",
                ImageFormats.Jpeg,
                -100,
                -200,
                FileStorageTypes.Local,
                cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenLargeDimensions_ShouldPassLargeValues()
    {
        // Arrange
        var command = new SetImagePreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Filename = "image.jpg",
            ImageFormat = ImageFormats.Jpeg,
            Width = int.MaxValue,
            Height = int.MaxValue,
            FileStorageTypeId = FileStorageTypes.Local
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };
        var imagePreviewDto = new ImagePreviewDto { MessageId = "message-123" };
        var accountIds = new List<string> { "account-1" };

        SetupBasicMocks(messageServiceModel, imagePreviewDto, accountIds, cancellationToken);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.SaveImagePreviewAsync(
                "message-123",
                "image.jpg",
                ImageFormats.Jpeg,
                int.MaxValue,
                int.MaxValue,
                FileStorageTypes.Local,
                cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenNullProperties_ShouldPassNullValues()
    {
        // Arrange
        var command = new SetImagePreviewCommand
        {
            MessageId = null,
            ChatId = null,
            Filename = null,
            ImageFormat = ImageFormats.Jpeg,
            Width = 800,
            Height = 600,
            FileStorageTypeId = FileStorageTypes.Local
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = null };
        var imagePreviewDto = new ImagePreviewDto { MessageId = "message-123" };
        var accountIds = new List<string> { "account-1" };

        _messageAgnosticServiceMock
            .Setup(x => x.SaveImagePreviewAsync(
                null!,
                null!,
                ImageFormats.Jpeg,
                800,
                600,
                FileStorageTypes.Local,
                cancellationToken))
            .ReturnsAsync(messageServiceModel);

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync(null!, cancellationToken))
            .ReturnsAsync(accountIds);

        _mapperMock
            .Setup(x => x.Map<ImagePreviewDto>(messageServiceModel))
            .Returns(imagePreviewDto);

        SetupAsyncOperations(cancellationToken);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.SaveImagePreviewAsync(
                null!,
                null!,
                ImageFormats.Jpeg,
                800,
                600,
                FileStorageTypes.Local,
                cancellationToken),
            Times.Once);

        _messageCacheManagerMock.Verify(
            x => x.ClearAsync(null!),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenEmptyAccountIdsList_ShouldNotPublishNotifications()
    {
        // Arrange
        var command = new SetImagePreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Filename = "image.jpg",
            ImageFormat = ImageFormats.Jpeg,
            Width = 800,
            Height = 600,
            FileStorageTypeId = FileStorageTypes.Local
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };
        var imagePreviewDto = new ImagePreviewDto { MessageId = "message-123" };
        var accountIds = new List<string>(); // Empty list

        SetupBasicMocks(messageServiceModel, imagePreviewDto, accountIds, cancellationToken);

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
    public async Task Handle_WhenMultipleAccountIds_ShouldPublishNotificationToEach()
    {
        // Arrange
        var command = new SetImagePreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Filename = "image.jpg",
            ImageFormat = ImageFormats.Jpeg,
            Width = 800,
            Height = 600,
            FileStorageTypeId = FileStorageTypes.Local
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };
        var imagePreviewDto = new ImagePreviewDto { MessageId = "message-123" };
        var accountIds = new List<string> { "account-1", "account-2", "account-3", "account-4" };

        SetupBasicMocks(messageServiceModel, imagePreviewDto, accountIds, cancellationToken);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        foreach (var accountId in accountIds)
        {
            _producerMock.Verify(
                x => x.PublishAsync(It.Is<Notification>(n =>
                    n.RecipientId == accountId &&
                    n.ImagePreview == imagePreviewDto), cancellationToken),
                Times.Once);
        }

        _producerMock.Verify(
            x => x.PublishAsync(It.IsAny<Notification>(), cancellationToken),
            Times.Exactly(4));
    }

    [Test]
    public void Handle_WhenMessageServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new SetImagePreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Filename = "image.jpg",
            ImageFormat = ImageFormats.Jpeg,
            Width = 800,
            Height = 600,
            FileStorageTypeId = FileStorageTypes.Local
        };
        var cancellationToken = CancellationToken.None;

        _messageAgnosticServiceMock
            .Setup(x => x.SaveImagePreviewAsync(
                "message-123",
                "image.jpg",
                ImageFormats.Jpeg,
                800,
                600,
                FileStorageTypes.Local,
                cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Message service error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));

        _chatAgnosticServiceMock.Verify(
            x => x.GetChatMemberAccountIdsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _mapperMock.Verify(
            x => x.Map<ImagePreviewDto>(It.IsAny<MessageServiceModel>()),
            Times.Never);

        _producerMock.Verify(
            x => x.PublishAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _messageCacheManagerMock.Verify(
            x => x.ClearAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Test]
    public void Handle_WhenChatServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new SetImagePreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Filename = "image.jpg",
            ImageFormat = ImageFormats.Jpeg,
            Width = 800,
            Height = 600,
            FileStorageTypeId = FileStorageTypes.Local
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };

        _messageAgnosticServiceMock
            .Setup(x => x.SaveImagePreviewAsync(
                "message-123",
                "image.jpg",
                ImageFormats.Jpeg,
                800,
                600,
                FileStorageTypes.Local,
                cancellationToken))
            .ReturnsAsync(messageServiceModel);

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Chat service error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));

        _messageAgnosticServiceMock.Verify(
            x => x.SaveImagePreviewAsync(
                "message-123",
                "image.jpg",
                ImageFormats.Jpeg,
                800,
                600,
                FileStorageTypes.Local,
                cancellationToken),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<ImagePreviewDto>(It.IsAny<MessageServiceModel>()),
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
        var command = new SetImagePreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Filename = "image.jpg",
            ImageFormat = ImageFormats.Jpeg,
            Width = 800,
            Height = 600,
            FileStorageTypeId = FileStorageTypes.Local
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };
        var accountIds = new List<string> { "account-1" };

        _messageAgnosticServiceMock
            .Setup(x => x.SaveImagePreviewAsync(
                "message-123",
                "image.jpg",
                ImageFormats.Jpeg,
                800,
                600,
                FileStorageTypes.Local,
                cancellationToken))
            .ReturnsAsync(messageServiceModel);

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(accountIds);

        _mapperMock
            .Setup(x => x.Map<ImagePreviewDto>(messageServiceModel))
            .Throws(new InvalidOperationException("Mapper error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));

        _messageAgnosticServiceMock.Verify(
            x => x.SaveImagePreviewAsync(
                "message-123",
                "image.jpg",
                ImageFormats.Jpeg,
                800,
                600,
                FileStorageTypes.Local,
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
        var command = new SetImagePreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Filename = "image.jpg",
            ImageFormat = ImageFormats.Jpeg,
            Width = 800,
            Height = 600,
            FileStorageTypeId = FileStorageTypes.Local
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };
        var imagePreviewDto = new ImagePreviewDto { MessageId = "message-123" };
        var accountIds = new List<string> { "account-1" };

        _messageAgnosticServiceMock
            .Setup(x => x.SaveImagePreviewAsync(
                "message-123",
                "image.jpg",
                ImageFormats.Jpeg,
                800,
                600,
                FileStorageTypes.Local,
                cancellationToken))
            .ReturnsAsync(messageServiceModel);

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(accountIds);

        _mapperMock
            .Setup(x => x.Map<ImagePreviewDto>(messageServiceModel))
            .Returns(imagePreviewDto);

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
        var command = new SetImagePreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Filename = "image.jpg",
            ImageFormat = ImageFormats.Jpeg,
            Width = 800,
            Height = 600,
            FileStorageTypeId = FileStorageTypes.Local
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };
        var imagePreviewDto = new ImagePreviewDto { MessageId = "message-123" };
        var accountIds = new List<string> { "account-1" };

        _messageAgnosticServiceMock
            .Setup(x => x.SaveImagePreviewAsync(
                "message-123",
                "image.jpg",
                ImageFormats.Jpeg,
                800,
                600,
                FileStorageTypes.Local,
                cancellationToken))
            .ReturnsAsync(messageServiceModel);

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync("chat-456", cancellationToken))
            .ReturnsAsync(accountIds);

        _mapperMock
            .Setup(x => x.Map<ImagePreviewDto>(messageServiceModel))
            .Returns(imagePreviewDto);

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
        var command = new SetImagePreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Filename = "image.jpg",
            ImageFormat = ImageFormats.Jpeg,
            Width = 800,
            Height = 600,
            FileStorageTypeId = FileStorageTypes.Local
        };
        var cancellationToken = new CancellationToken(false);

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };
        var imagePreviewDto = new ImagePreviewDto { MessageId = "message-123" };
        var accountIds = new List<string> { "account-1" };

        SetupBasicMocks(messageServiceModel, imagePreviewDto, accountIds, cancellationToken);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _messageAgnosticServiceMock.Verify(
            x => x.SaveImagePreviewAsync(
                "message-123",
                "image.jpg",
                ImageFormats.Jpeg,
                800,
                600,
                FileStorageTypes.Local,
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
    public async Task Handle_WhenSpecialCharactersInFilename_ShouldHandleCorrectly()
    {
        // Arrange
        var command = new SetImagePreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Filename = "image with spaces & special chars!@#$%^&*().jpg",
            ImageFormat = ImageFormats.Jpeg,
            Width = 800,
            Height = 600,
            FileStorageTypeId = FileStorageTypes.Local
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };
        var imagePreviewDto = new ImagePreviewDto { MessageId = "message-123" };
        var accountIds = new List<string> { "account-1" };

        SetupBasicMocks(messageServiceModel, imagePreviewDto, accountIds, cancellationToken);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.SaveImagePreviewAsync(
                "message-123",
                "image with spaces & special chars!@#$%^&*().jpg",
                ImageFormats.Jpeg,
                800,
                600,
                FileStorageTypes.Local,
                cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenUnicodeCharactersInFilename_ShouldHandleCorrectly()
    {
        // Arrange
        var command = new SetImagePreviewCommand
        {
            MessageId = "message-123",
            ChatId = "chat-456",
            Filename = "图片-🖼️-صورة.jpg",
            ImageFormat = ImageFormats.Jpeg,
            Width = 800,
            Height = 600,
            FileStorageTypeId = FileStorageTypes.Local
        };
        var cancellationToken = CancellationToken.None;

        var messageServiceModel = new MessageServiceModel { Id = "message-123", ChatId = "chat-456" };
        var imagePreviewDto = new ImagePreviewDto { MessageId = "message-123" };
        var accountIds = new List<string> { "account-1" };

        SetupBasicMocks(messageServiceModel, imagePreviewDto, accountIds, cancellationToken);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);

        _messageAgnosticServiceMock.Verify(
            x => x.SaveImagePreviewAsync(
                "message-123",
                "图片-🖼️-صورة.jpg",
                ImageFormats.Jpeg,
                800,
                600,
                FileStorageTypes.Local,
                cancellationToken),
            Times.Once);
    }

    private void SetupBasicMocks(MessageServiceModel messageServiceModel, ImagePreviewDto imagePreviewDto, List<string> accountIds, CancellationToken cancellationToken)
    {
        _messageAgnosticServiceMock
            .Setup(x => x.SaveImagePreviewAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<ImageFormats>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<FileStorageTypes>(),
                cancellationToken))
            .ReturnsAsync(messageServiceModel);

        _chatAgnosticServiceMock
            .Setup(x => x.GetChatMemberAccountIdsAsync(It.IsAny<string>(), cancellationToken))
            .ReturnsAsync(accountIds);

        _mapperMock
            .Setup(x => x.Map<ImagePreviewDto>(messageServiceModel))
            .Returns(imagePreviewDto);

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
}
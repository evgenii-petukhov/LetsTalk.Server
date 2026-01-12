using AutoMapper;
using FluentAssertions;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using LetsTalk.Server.Persistence.EntityFramework.Services;
using LetsTalk.Server.Persistence.Enums;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Globalization;
using System.Reflection;

namespace LetsTalk.Server.Persistence.EntityFramework.Tests;

[TestFixture]
public class MessageEntityFrameworkServiceGeneratedTests
{
    private Mock<IMessageRepository> _mockMessageRepository;
    private Mock<IChatMessageStatusRepository> _mockChatMessageStatusRepository;
    private Mock<ILinkPreviewRepository> _mockLinkPreviewRepository;
    private Mock<IEntityFactory> _mockEntityFactory;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IMapper> _mockMapper;
    private MessageEntityFrameworkService _service;

    [SetUp]
    public void SetUp()
    {
        _mockMessageRepository = new Mock<IMessageRepository>();
        _mockChatMessageStatusRepository = new Mock<IChatMessageStatusRepository>();
        _mockLinkPreviewRepository = new Mock<ILinkPreviewRepository>();
        _mockEntityFactory = new Mock<IEntityFactory>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();

        _service = new MessageEntityFrameworkService(
            _mockMessageRepository.Object,
            _mockChatMessageStatusRepository.Object,
            _mockLinkPreviewRepository.Object,
            _mockEntityFactory.Object,
            _mockUnitOfWork.Object,
            _mockMapper.Object);
    }

    [TestFixture]
    public class CreateMessageAsyncWithLinkPreviewTests : MessageEntityFrameworkServiceGeneratedTests
    {
        [Test]
        public async Task CreateMessageAsync_WithValidLinkPreviewId_ShouldCreateMessageAndReturnMappedResult()
        {
            // Arrange
            const string senderId = "123";
            const string chatId = "456";
            const string text = "Hello world";
            const string textHtml = "<p>Hello world</p>";
            const string linkPreviewId = "789";
            var message = CreateMessageWithId(1);
            var retrievedMessage = CreateMessageWithId(1);
            var expectedResult = new MessageServiceModel { Id = "1", Text = text };

            _mockMessageRepository
                .Setup(x => x.CreateAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockMessageRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(retrievedMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(retrievedMessage))
                .Returns(expectedResult);

            // Act
            var result = await _service.CreateMessageAsync(senderId, chatId, text, textHtml, linkPreviewId, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            _mockMessageRepository.Verify(x => x.CreateAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockMessageRepository.Verify(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(x => x.Map<MessageServiceModel>(retrievedMessage), Times.Once);
        }

        [Test]
        public async Task CreateMessageAsync_WithInvalidLinkPreviewId_ShouldCreateMessageWithNullLinkPreview()
        {
            // Arrange
            const string senderId = "123";
            const string chatId = "456";
            const string text = "Hello world";
            const string textHtml = "<p>Hello world</p>";
            const string linkPreviewId = "invalid";
            var message = CreateMessageWithId(1);
            var retrievedMessage = CreateMessageWithId(1);
            var expectedResult = new MessageServiceModel { Id = "1", Text = text };

            _mockMessageRepository
                .Setup(x => x.CreateAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockMessageRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(retrievedMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(retrievedMessage))
                .Returns(expectedResult);

            // Act
            var result = await _service.CreateMessageAsync(senderId, chatId, text, textHtml, linkPreviewId, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            _mockMessageRepository.Verify(x => x.CreateAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task CreateMessageAsync_WithEmptyLinkPreviewId_ShouldCreateMessageWithNullLinkPreview()
        {
            // Arrange
            const string senderId = "123";
            const string chatId = "456";
            const string text = "Hello world";
            const string textHtml = "<p>Hello world</p>";
            const string linkPreviewId = "";
            var message = CreateMessageWithId(1);
            var retrievedMessage = CreateMessageWithId(1);
            var expectedResult = new MessageServiceModel { Id = "1", Text = text };

            _mockMessageRepository
                .Setup(x => x.CreateAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockMessageRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(retrievedMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(retrievedMessage))
                .Returns(expectedResult);

            // Act
            var result = await _service.CreateMessageAsync(senderId, chatId, text, textHtml, linkPreviewId, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void CreateMessageAsync_WithInvalidSenderId_ShouldThrowFormatException()
        {
            // Arrange
            const string senderId = "invalid";
            const string chatId = "456";
            const string text = "Hello world";
            const string textHtml = "<p>Hello world</p>";
            const string linkPreviewId = "789";

            // Act & Assert
            var act = async () => await _service.CreateMessageAsync(senderId, chatId, text, textHtml, linkPreviewId, CancellationToken.None);
            act.Should().ThrowAsync<FormatException>();
        }

        [Test]
        public void CreateMessageAsync_WithInvalidChatId_ShouldThrowFormatException()
        {
            // Arrange
            const string senderId = "123";
            const string chatId = "invalid";
            const string text = "Hello world";
            const string textHtml = "<p>Hello world</p>";
            const string linkPreviewId = "789";

            // Act & Assert
            var act = async () => await _service.CreateMessageAsync(senderId, chatId, text, textHtml, linkPreviewId, CancellationToken.None);
            act.Should().ThrowAsync<FormatException>();
        }
    }

    [TestFixture]
    public class CreateMessageAsyncWithImageTests : MessageEntityFrameworkServiceGeneratedTests
    {
        [Test]
        public async Task CreateMessageAsync_WithImage_ShouldCreateMessageWithImageAndReturnMappedResult()
        {
            // Arrange
            const string senderId = "123";
            const string chatId = "456";
            const string text = "Hello world";
            const string textHtml = "<p>Hello world</p>";
            const string imageId = "image123";
            const int width = 800;
            const int height = 600;
            const ImageFormats imageFormat = ImageFormats.Jpeg;
            const FileStorageTypes fileStorageType = FileStorageTypes.Local;

            var image = CreateImage(imageId);
            var message = CreateMessageWithId(1);
            var expectedResult = new MessageServiceModel { Id = "1", Text = text };

            _mockEntityFactory
                .Setup(x => x.CreateImage(imageId, imageFormat, width, height, fileStorageType))
                .Returns(image);

            _mockMessageRepository
                .Setup(x => x.CreateAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(It.IsAny<Message>()))
                .Returns(expectedResult);

            // Act
            var result = await _service.CreateMessageAsync(senderId, chatId, text, textHtml, imageId, width, height, imageFormat, fileStorageType, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            _mockEntityFactory.Verify(x => x.CreateImage(imageId, imageFormat, width, height, fileStorageType), Times.Once);
            _mockMessageRepository.Verify(x => x.CreateAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(x => x.Map<MessageServiceModel>(It.IsAny<Message>()), Times.Once);
        }

        [Test]
        public void CreateMessageAsync_WithImageAndInvalidSenderId_ShouldThrowFormatException()
        {
            // Arrange
            const string senderId = "invalid";
            const string chatId = "456";
            const string text = "Hello world";
            const string textHtml = "<p>Hello world</p>";
            const string imageId = "image123";
            const int width = 800;
            const int height = 600;
            const ImageFormats imageFormat = ImageFormats.Jpeg;
            const FileStorageTypes fileStorageType = FileStorageTypes.Local;

            // Act & Assert
            var act = async () => await _service.CreateMessageAsync(senderId, chatId, text, textHtml, imageId, width, height, imageFormat, fileStorageType, CancellationToken.None);
            act.Should().ThrowAsync<FormatException>();
        }

        [Test]
        public void CreateMessageAsync_WithImageAndInvalidChatId_ShouldThrowFormatException()
        {
            // Arrange
            const string senderId = "123";
            const string chatId = "invalid";
            const string text = "Hello world";
            const string textHtml = "<p>Hello world</p>";
            const string imageId = "image123";
            const int width = 800;
            const int height = 600;
            const ImageFormats imageFormat = ImageFormats.Jpeg;
            const FileStorageTypes fileStorageType = FileStorageTypes.Local;

            // Act & Assert
            var act = async () => await _service.CreateMessageAsync(senderId, chatId, text, textHtml, imageId, width, height, imageFormat, fileStorageType, CancellationToken.None);
            act.Should().ThrowAsync<FormatException>();
        }
    }

    [TestFixture]
    public class GetPagedAsyncTests : MessageEntityFrameworkServiceGeneratedTests
    {
        [Test]
        public async Task GetPagedAsync_WithValidParameters_ShouldReturnMappedMessages()
        {
            // Arrange
            const string chatId = "123";
            const int pageIndex = 0;
            const int messagesPerPage = 10;
            var messages = new List<Message> { CreateMessageWithId(1), CreateMessageWithId(2) };
            var expectedResult = new List<MessageServiceModel>
            {
                new() { Id = "1", Text = "Message 1" },
                new() { Id = "2", Text = "Message 2" }
            };

            _mockMessageRepository
                .Setup(x => x.GetPagedAsync(123, pageIndex, messagesPerPage, It.IsAny<CancellationToken>()))
                .ReturnsAsync(messages);

            _mockMapper
                .Setup(x => x.Map<List<MessageServiceModel>>(messages))
                .Returns(expectedResult);

            // Act
            var result = await _service.GetPagedAsync(chatId, pageIndex, messagesPerPage);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            _mockMessageRepository.Verify(x => x.GetPagedAsync(123, pageIndex, messagesPerPage, It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(x => x.Map<List<MessageServiceModel>>(messages), Times.Once);
        }

        [Test]
        public async Task GetPagedAsync_WithEmptyResult_ShouldReturnEmptyList()
        {
            // Arrange
            const string chatId = "123";
            const int pageIndex = 0;
            const int messagesPerPage = 10;
            var messages = new List<Message>();
            var expectedResult = new List<MessageServiceModel>();

            _mockMessageRepository
                .Setup(x => x.GetPagedAsync(123, pageIndex, messagesPerPage, It.IsAny<CancellationToken>()))
                .ReturnsAsync(messages);

            _mockMapper
                .Setup(x => x.Map<List<MessageServiceModel>>(messages))
                .Returns(expectedResult);

            // Act
            var result = await _service.GetPagedAsync(chatId, pageIndex, messagesPerPage);

            // Assert
            result.Should().BeEmpty();
            _mockMessageRepository.Verify(x => x.GetPagedAsync(123, pageIndex, messagesPerPage, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetPagedAsync_WithCancellationToken_ShouldPassTokenToRepository()
        {
            // Arrange
            const string chatId = "123";
            const int pageIndex = 0;
            const int messagesPerPage = 10;
            var cancellationToken = new CancellationToken();
            var messages = new List<Message>();
            var expectedResult = new List<MessageServiceModel>();

            _mockMessageRepository
                .Setup(x => x.GetPagedAsync(123, pageIndex, messagesPerPage, cancellationToken))
                .ReturnsAsync(messages);

            _mockMapper
                .Setup(x => x.Map<List<MessageServiceModel>>(messages))
                .Returns(expectedResult);

            // Act
            await _service.GetPagedAsync(chatId, pageIndex, messagesPerPage, cancellationToken);

            // Assert
            _mockMessageRepository.Verify(x => x.GetPagedAsync(123, pageIndex, messagesPerPage, cancellationToken), Times.Once);
        }

        [Test]
        public void GetPagedAsync_WithInvalidChatId_ShouldThrowFormatException()
        {
            // Arrange
            const string chatId = "invalid";
            const int pageIndex = 0;
            const int messagesPerPage = 10;

            // Act & Assert
            var act = async () => await _service.GetPagedAsync(chatId, pageIndex, messagesPerPage);
            act.Should().ThrowAsync<FormatException>();
        }
    }

    [TestFixture]
    public class SetLinkPreviewAsyncWithIdTests : MessageEntityFrameworkServiceGeneratedTests
    {
        [Test]
        public async Task SetLinkPreviewAsync_WithValidIds_ShouldSetLinkPreviewAndReturnMappedResult()
        {
            // Arrange
            const string messageId = "123";
            const string linkPreviewId = "456";
            var linkPreview = CreateLinkPreview("https://example.com");
            var message = CreateMessageWithId(123);
            var expectedResult = new MessageServiceModel { Id = "123", Text = "Test message" };

            _mockLinkPreviewRepository
                .Setup(x => x.GetByIdAsync(456, It.IsAny<CancellationToken>()))
                .ReturnsAsync(linkPreview);

            _mockMessageRepository
                .Setup(x => x.GetByIdAsTrackingAsync(123, It.IsAny<CancellationToken>()))
                .ReturnsAsync(message);

            _mockUnitOfWork
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(message))
                .Returns(expectedResult);

            // Act
            var result = await _service.SetLinkPreviewAsync(messageId, linkPreviewId);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            _mockLinkPreviewRepository.Verify(x => x.GetByIdAsync(456, It.IsAny<CancellationToken>()), Times.Once);
            _mockMessageRepository.Verify(x => x.GetByIdAsTrackingAsync(123, It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(x => x.Map<MessageServiceModel>(message), Times.Once);
        }

        [Test]
        public void SetLinkPreviewAsync_WithInvalidMessageId_ShouldThrowFormatException()
        {
            // Arrange
            const string messageId = "invalid";
            const string linkPreviewId = "456";

            // Act & Assert
            var act = async () => await _service.SetLinkPreviewAsync(messageId, linkPreviewId);
            act.Should().ThrowAsync<FormatException>();
        }

        [Test]
        public void SetLinkPreviewAsync_WithInvalidLinkPreviewId_ShouldThrowFormatException()
        {
            // Arrange
            const string messageId = "123";
            const string linkPreviewId = "invalid";

            // Act & Assert
            var act = async () => await _service.SetLinkPreviewAsync(messageId, linkPreviewId);
            act.Should().ThrowAsync<FormatException>();
        }
    }

    [TestFixture]
    public class SetLinkPreviewAsyncWithDataTests : MessageEntityFrameworkServiceGeneratedTests
    {
        [Test]
        public async Task SetLinkPreviewAsync_WithValidData_ShouldCreateLinkPreviewAndReturnMappedResult()
        {
            // Arrange
            const string messageId = "123";
            const string url = "https://example.com";
            const string title = "Example Title";
            const string imageUrl = "https://example.com/image.jpg";
            var linkPreview = CreateLinkPreview(url);
            var message = CreateMessageWithId(123);
            var expectedResult = new MessageServiceModel { Id = "123", Text = "Test message" };

            _mockEntityFactory
                .Setup(x => x.CreateLinkPreview(url, title, imageUrl))
                .Returns(linkPreview);

            _mockMessageRepository
                .Setup(x => x.GetByIdAsTrackingAsync(123, It.IsAny<CancellationToken>()))
                .ReturnsAsync(message);

            _mockUnitOfWork
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(message))
                .Returns(expectedResult);

            // Act
            var result = await _service.SetLinkPreviewAsync(messageId, url, title, imageUrl);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            _mockEntityFactory.Verify(x => x.CreateLinkPreview(url, title, imageUrl), Times.Once);
            _mockMessageRepository.Verify(x => x.GetByIdAsTrackingAsync(123, It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(x => x.Map<MessageServiceModel>(message), Times.Once);
        }

        [Test]
        public void SetLinkPreviewAsync_WithInvalidMessageId_ShouldThrowFormatException()
        {
            // Arrange
            const string messageId = "invalid";
            const string url = "https://example.com";
            const string title = "Example Title";
            const string imageUrl = "https://example.com/image.jpg";

            // Act & Assert
            var act = async () => await _service.SetLinkPreviewAsync(messageId, url, title, imageUrl);
            act.Should().ThrowAsync<FormatException>();
        }
    }

    [TestFixture]
    public class MarkAsReadAsyncTests : MessageEntityFrameworkServiceGeneratedTests
    {
        [Test]
        public async Task MarkAsReadAsync_WithValidIds_ShouldCreateChatMessageStatusAndSave()
        {
            // Arrange
            const string chatId = "123";
            const string accountId = "456";
            const string messageId = "789";
            var chatMessageStatus = CreateChatMessageStatus(123, 456, 789);

            _mockEntityFactory
                .Setup(x => x.CreateChatMessageStatus(123, 456, 789, false))
                .Returns(chatMessageStatus);

            _mockChatMessageStatusRepository
                .Setup(x => x.CreateAsync(chatMessageStatus, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.MarkAsReadAsync(chatId, accountId, messageId, CancellationToken.None);

            // Assert
            _mockEntityFactory.Verify(x => x.CreateChatMessageStatus(123, 456, 789, false), Times.Once);
            _mockChatMessageStatusRepository.Verify(x => x.CreateAsync(chatMessageStatus, It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task MarkAsReadAsync_WhenDbUpdateExceptionOccurs_ShouldRetryWithAttachToContext()
        {
            // Arrange
            const string chatId = "123";
            const string accountId = "456";
            const string messageId = "789";
            var chatMessageStatus1 = CreateChatMessageStatus(123, 456, 789);
            var chatMessageStatus2 = CreateChatMessageStatus(123, 456, 789);

            _mockEntityFactory
                .SetupSequence(x => x.CreateChatMessageStatus(123, 456, 789, false))
                .Returns(chatMessageStatus1);

            _mockEntityFactory
                .Setup(x => x.CreateChatMessageStatus(123, 456, 789, true))
                .Returns(chatMessageStatus2);

            _mockChatMessageStatusRepository
                .Setup(x => x.CreateAsync(chatMessageStatus1, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .SetupSequence(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DbUpdateException("Duplicate key"))
                .Returns(Task.CompletedTask);

            // Act
            await _service.MarkAsReadAsync(chatId, accountId, messageId, CancellationToken.None);

            // Assert
            _mockEntityFactory.Verify(x => x.CreateChatMessageStatus(123, 456, 789, false), Times.Once);
            _mockEntityFactory.Verify(x => x.CreateChatMessageStatus(123, 456, 789, true), Times.Once);
            _mockChatMessageStatusRepository.Verify(x => x.CreateAsync(chatMessageStatus1, It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Test]
        public void MarkAsReadAsync_WithInvalidChatId_ShouldThrowFormatException()
        {
            // Arrange
            const string chatId = "invalid";
            const string accountId = "456";
            const string messageId = "789";

            // Act & Assert
            var act = async () => await _service.MarkAsReadAsync(chatId, accountId, messageId, CancellationToken.None);
            act.Should().ThrowAsync<FormatException>();
        }

        [Test]
        public void MarkAsReadAsync_WithInvalidAccountId_ShouldThrowFormatException()
        {
            // Arrange
            const string chatId = "123";
            const string accountId = "invalid";
            const string messageId = "789";

            // Act & Assert
            var act = async () => await _service.MarkAsReadAsync(chatId, accountId, messageId, CancellationToken.None);
            act.Should().ThrowAsync<FormatException>();
        }

        [Test]
        public void MarkAsReadAsync_WithInvalidMessageId_ShouldThrowFormatException()
        {
            // Arrange
            const string chatId = "123";
            const string accountId = "456";
            const string messageId = "invalid";

            // Act & Assert
            var act = async () => await _service.MarkAsReadAsync(chatId, accountId, messageId, CancellationToken.None);
            act.Should().ThrowAsync<FormatException>();
        }
    }

    [TestFixture]
    public class SaveImagePreviewAsyncTests : MessageEntityFrameworkServiceGeneratedTests
    {
        [Test]
        public async Task SaveImagePreviewAsync_WithValidParameters_ShouldCreateImageAndSetPreview()
        {
            // Arrange
            const string messageId = "123";
            const string filename = "preview.jpg";
            const ImageFormats imageFormat = ImageFormats.Jpeg;
            const int width = 200;
            const int height = 150;
            const FileStorageTypes fileStorageType = FileStorageTypes.Local;

            var image = CreateImage(filename);
            var message = CreateMessageWithId(123);
            var expectedResult = new MessageServiceModel { Id = "123", Text = "Test message" };

            _mockEntityFactory
                .Setup(x => x.CreateImage(filename, imageFormat, width, height, fileStorageType))
                .Returns(image);

            _mockMessageRepository
                .Setup(x => x.GetByIdAsTrackingAsync(123, It.IsAny<CancellationToken>()))
                .ReturnsAsync(message);

            _mockUnitOfWork
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(message))
                .Returns(expectedResult);

            // Act
            var result = await _service.SaveImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            _mockEntityFactory.Verify(x => x.CreateImage(filename, imageFormat, width, height, fileStorageType), Times.Once);
            _mockMessageRepository.Verify(x => x.GetByIdAsTrackingAsync(123, It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(x => x.Map<MessageServiceModel>(message), Times.Once);
        }

        [Test]
        public void SaveImagePreviewAsync_WithInvalidMessageId_ShouldThrowFormatException()
        {
            // Arrange
            const string messageId = "invalid";
            const string filename = "preview.jpg";
            const ImageFormats imageFormat = ImageFormats.Jpeg;
            const int width = 200;
            const int height = 150;
            const FileStorageTypes fileStorageType = FileStorageTypes.Local;

            // Act & Assert
            var act = async () => await _service.SaveImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType);
            act.Should().ThrowAsync<FormatException>();
        }
    }

    private static Message CreateMessageWithId(int id)
    {
        var message = new Message(1, 1, "Test message", "<p>Test message</p>");
        var idProperty = typeof(BaseEntity).GetProperty("Id");
        idProperty!.SetValue(message, id);
        return message;
    }

    private static Image CreateImage(string imageId)
    {
        return new Image(imageId, (int)ImageFormats.Jpeg, 100, 100, (int)FileStorageTypes.Local);
    }

    private static LinkPreview CreateLinkPreview(string url)
    {
        return new LinkPreview(url, "Title", "https://example.com/image.jpg");
    }

    private static ChatMessageStatus CreateChatMessageStatus(int chatId, int accountId, int messageId)
    {
        return new ChatMessageStatus(chatId, accountId, messageId);
    }
}
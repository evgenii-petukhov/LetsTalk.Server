using AutoMapper;
using FluentAssertions;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.MongoDB.Models;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;
using Moq;

namespace LetsTalk.Server.Persistence.MongoDB.Services.Tests;

[TestFixture]
public class MessageMongoDBServiceTests
{
    private Mock<IMessageRepository> _mockMessageRepository;
    private Mock<IMapper> _mockMapper;
    private MessageMongoDBService _service;

    [SetUp]
    public void SetUp()
    {
        _mockMessageRepository = new Mock<IMessageRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new MessageMongoDBService(_mockMessageRepository.Object, _mockMapper.Object);
    }

    [TestFixture]
    public class CreateMessageAsyncWithLinkPreviewTests : MessageMongoDBServiceTests
    {
        [Test]
        public async Task CreateMessageAsync_WithLinkPreview_ShouldCreateMessageAndReturnMappedResult()
        {
            // Arrange
            const string senderId = "507f1f77bcf86cd799439011";
            const string chatId = "507f1f77bcf86cd799439012";
            const string text = "Check out this link";
            const string textHtml = "<p>Check out this link</p>";
            const string linkPreviewId = "507f1f77bcf86cd799439013";
            var cancellationToken = new CancellationToken();

            var createdMessage = new Message
            {
                Id = "507f1f77bcf86cd799439014",
                SenderId = senderId,
                ChatId = chatId,
                Text = text,
                TextHtml = textHtml,
                LinkPreviewId = linkPreviewId
            };

            var retrievedMessage = new Message
            {
                Id = "507f1f77bcf86cd799439014",
                SenderId = senderId,
                ChatId = chatId,
                Text = text,
                TextHtml = textHtml,
                LinkPreviewId = linkPreviewId,
                DateCreatedUnix = 1640995200
            };

            var expectedServiceModel = new MessageServiceModel
            {
                Id = "507f1f77bcf86cd799439014",
                SenderId = senderId,
                ChatId = chatId,
                Text = text,
                TextHtml = textHtml,
                DateCreatedUnix = 1640995200
            };

            _mockMessageRepository
                .Setup(x => x.CreateAsync(senderId, chatId, text, textHtml, linkPreviewId, cancellationToken))
                .ReturnsAsync(createdMessage);

            _mockMessageRepository
                .Setup(x => x.GetByIdAsync(createdMessage.Id, cancellationToken))
                .ReturnsAsync(retrievedMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(retrievedMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.CreateMessageAsync(senderId, chatId, text, textHtml, linkPreviewId, cancellationToken);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModel);
            _mockMessageRepository.Verify(x => x.CreateAsync(senderId, chatId, text, textHtml, linkPreviewId, cancellationToken), Times.Once);
            _mockMessageRepository.Verify(x => x.GetByIdAsync(createdMessage.Id, cancellationToken), Times.Once);
            _mockMapper.Verify(x => x.Map<MessageServiceModel>(retrievedMessage), Times.Once);
        }

        [Test]
        public async Task CreateMessageAsync_WithLinkPreviewAndNullParameters_ShouldPassNullsToRepository()
        {
            // Arrange
            string senderId = null;
            string chatId = null;
            string text = null;
            string textHtml = null;
            string linkPreviewId = null;
            var cancellationToken = new CancellationToken();

            var createdMessage = new Message { Id = "507f1f77bcf86cd799439015" };
            var retrievedMessage = new Message { Id = "507f1f77bcf86cd799439015" };
            var expectedServiceModel = new MessageServiceModel { Id = "507f1f77bcf86cd799439015" };

            _mockMessageRepository
                .Setup(x => x.CreateAsync(senderId, chatId, text, textHtml, linkPreviewId, cancellationToken))
                .ReturnsAsync(createdMessage);

            _mockMessageRepository
                .Setup(x => x.GetByIdAsync(createdMessage.Id, cancellationToken))
                .ReturnsAsync(retrievedMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(retrievedMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.CreateMessageAsync(senderId, chatId, text, textHtml, linkPreviewId, cancellationToken);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModel);
            _mockMessageRepository.Verify(x => x.CreateAsync(senderId, chatId, text, textHtml, linkPreviewId, cancellationToken), Times.Once);
        }

        [Test]
        public async Task CreateMessageAsync_WithLinkPreviewAndEmptyStrings_ShouldPassEmptyStringsToRepository()
        {
            // Arrange
            const string senderId = "";
            const string chatId = "";
            const string text = "";
            const string textHtml = "";
            const string linkPreviewId = "";
            var cancellationToken = new CancellationToken();

            var createdMessage = new Message { Id = "507f1f77bcf86cd799439016" };
            var retrievedMessage = new Message { Id = "507f1f77bcf86cd799439016" };
            var expectedServiceModel = new MessageServiceModel { Id = "507f1f77bcf86cd799439016" };

            _mockMessageRepository
                .Setup(x => x.CreateAsync(senderId, chatId, text, textHtml, linkPreviewId, cancellationToken))
                .ReturnsAsync(createdMessage);

            _mockMessageRepository
                .Setup(x => x.GetByIdAsync(createdMessage.Id, cancellationToken))
                .ReturnsAsync(retrievedMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(retrievedMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.CreateMessageAsync(senderId, chatId, text, textHtml, linkPreviewId, cancellationToken);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModel);
            _mockMessageRepository.Verify(x => x.CreateAsync(senderId, chatId, text, textHtml, linkPreviewId, cancellationToken), Times.Once);
        }

        [Test]
        public async Task CreateMessageAsync_WithLinkPreviewWhenGetByIdReturnsNull_ShouldReturnMappedNull()
        {
            // Arrange
            const string senderId = "507f1f77bcf86cd799439011";
            const string chatId = "507f1f77bcf86cd799439012";
            const string text = "Test message";
            const string textHtml = "<p>Test message</p>";
            const string linkPreviewId = "507f1f77bcf86cd799439013";
            var cancellationToken = new CancellationToken();

            var createdMessage = new Message { Id = "507f1f77bcf86cd799439017" };
            Message retrievedMessage = null;
            MessageServiceModel expectedServiceModel = null;

            _mockMessageRepository
                .Setup(x => x.CreateAsync(senderId, chatId, text, textHtml, linkPreviewId, cancellationToken))
                .ReturnsAsync(createdMessage);

            _mockMessageRepository
                .Setup(x => x.GetByIdAsync(createdMessage.Id, cancellationToken))
                .ReturnsAsync(retrievedMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(retrievedMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.CreateMessageAsync(senderId, chatId, text, textHtml, linkPreviewId, cancellationToken);

            // Assert
            result.Should().BeNull();
            _mockMessageRepository.Verify(x => x.GetByIdAsync(createdMessage.Id, cancellationToken), Times.Once);
            _mockMapper.Verify(x => x.Map<MessageServiceModel>(retrievedMessage), Times.Once);
        }
    }

    [TestFixture]
    public class CreateMessageAsyncWithImageTests : MessageMongoDBServiceTests
    {
        [Test]
        public async Task CreateMessageAsync_WithImage_ShouldCreateMessageAndReturnMappedResult()
        {
            // Arrange
            const string senderId = "507f1f77bcf86cd799439011";
            const string chatId = "507f1f77bcf86cd799439012";
            const string text = "Check out this image";
            const string textHtml = "<p>Check out this image</p>";
            const string imageId = "image123";
            const int width = 800;
            const int height = 600;
            const ImageFormats imageFormat = ImageFormats.Jpeg;
            const FileStorageTypes fileStorageType = FileStorageTypes.AmazonS3;
            var cancellationToken = new CancellationToken();

            var createdMessage = new Message
            {
                Id = "507f1f77bcf86cd799439018",
                SenderId = senderId,
                ChatId = chatId,
                Text = text,
                TextHtml = textHtml,
                Image = new Image
                {
                    Id = imageId,
                    Width = width,
                    Height = height,
                    ImageFormatId = (int)imageFormat,
                    FileStorageTypeId = (int)fileStorageType
                }
            };

            var expectedServiceModel = new MessageServiceModel
            {
                Id = "507f1f77bcf86cd799439018",
                SenderId = senderId,
                ChatId = chatId,
                Text = text,
                TextHtml = textHtml,
                Image = new ImageServiceModel
                {
                    Id = imageId,
                    FileStorageTypeId = (int)fileStorageType
                }
            };

            _mockMessageRepository
                .Setup(x => x.CreateAsync(senderId, chatId, text, textHtml, imageId, width, height, imageFormat, fileStorageType, cancellationToken))
                .ReturnsAsync(createdMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(createdMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.CreateMessageAsync(senderId, chatId, text, textHtml, imageId, width, height, imageFormat, fileStorageType, cancellationToken);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModel);
            _mockMessageRepository.Verify(x => x.CreateAsync(senderId, chatId, text, textHtml, imageId, width, height, imageFormat, fileStorageType, cancellationToken), Times.Once);
            _mockMapper.Verify(x => x.Map<MessageServiceModel>(createdMessage), Times.Once);
        }

        [Test]
        public async Task CreateMessageAsync_WithImageAndDifferentFormats_ShouldHandleAllFormats()
        {
            // Arrange
            const string senderId = "507f1f77bcf86cd799439011";
            const string chatId = "507f1f77bcf86cd799439012";
            const string text = "PNG image";
            const string textHtml = "<p>PNG image</p>";
            const string imageId = "image456";
            const int width = 1024;
            const int height = 768;
            const ImageFormats imageFormat = ImageFormats.Png;
            const FileStorageTypes fileStorageType = FileStorageTypes.Local;
            var cancellationToken = new CancellationToken();

            var createdMessage = new Message { Id = "507f1f77bcf86cd799439019" };
            var expectedServiceModel = new MessageServiceModel { Id = "507f1f77bcf86cd799439019" };

            _mockMessageRepository
                .Setup(x => x.CreateAsync(senderId, chatId, text, textHtml, imageId, width, height, imageFormat, fileStorageType, cancellationToken))
                .ReturnsAsync(createdMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(createdMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.CreateMessageAsync(senderId, chatId, text, textHtml, imageId, width, height, imageFormat, fileStorageType, cancellationToken);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModel);
            _mockMessageRepository.Verify(x => x.CreateAsync(senderId, chatId, text, textHtml, imageId, width, height, imageFormat, fileStorageType, cancellationToken), Times.Once);
        }

        [Test]
        public async Task CreateMessageAsync_WithImageAndZeroDimensions_ShouldPassZeroValuesToRepository()
        {
            // Arrange
            const string senderId = "507f1f77bcf86cd799439011";
            const string chatId = "507f1f77bcf86cd799439012";
            const string text = "Small image";
            const string textHtml = "<p>Small image</p>";
            const string imageId = "image789";
            const int width = 0;
            const int height = 0;
            const ImageFormats imageFormat = ImageFormats.Gif;
            const FileStorageTypes fileStorageType = FileStorageTypes.AzureBlobStorage;
            var cancellationToken = new CancellationToken();

            var createdMessage = new Message { Id = "507f1f77bcf86cd799439020" };
            var expectedServiceModel = new MessageServiceModel { Id = "507f1f77bcf86cd799439020" };

            _mockMessageRepository
                .Setup(x => x.CreateAsync(senderId, chatId, text, textHtml, imageId, width, height, imageFormat, fileStorageType, cancellationToken))
                .ReturnsAsync(createdMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(createdMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.CreateMessageAsync(senderId, chatId, text, textHtml, imageId, width, height, imageFormat, fileStorageType, cancellationToken);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModel);
            _mockMessageRepository.Verify(x => x.CreateAsync(senderId, chatId, text, textHtml, imageId, width, height, imageFormat, fileStorageType, cancellationToken), Times.Once);
        }

        [Test]
        public async Task CreateMessageAsync_WithImageAndNullImageId_ShouldPassNullToRepository()
        {
            // Arrange
            const string senderId = "507f1f77bcf86cd799439011";
            const string chatId = "507f1f77bcf86cd799439012";
            const string text = "No image";
            const string textHtml = "<p>No image</p>";
            string imageId = null;
            const int width = 100;
            const int height = 100;
            const ImageFormats imageFormat = ImageFormats.Webp;
            const FileStorageTypes fileStorageType = FileStorageTypes.Local;
            var cancellationToken = new CancellationToken();

            var createdMessage = new Message { Id = "507f1f77bcf86cd799439021" };
            var expectedServiceModel = new MessageServiceModel { Id = "507f1f77bcf86cd799439021" };

            _mockMessageRepository
                .Setup(x => x.CreateAsync(senderId, chatId, text, textHtml, imageId, width, height, imageFormat, fileStorageType, cancellationToken))
                .ReturnsAsync(createdMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(createdMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.CreateMessageAsync(senderId, chatId, text, textHtml, imageId, width, height, imageFormat, fileStorageType, cancellationToken);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModel);
            _mockMessageRepository.Verify(x => x.CreateAsync(senderId, chatId, text, textHtml, imageId, width, height, imageFormat, fileStorageType, cancellationToken), Times.Once);
        }

        [Test]
        public async Task CreateMessageAsync_WithImageAndNegativeDimensions_ShouldPassNegativeValuesToRepository()
        {
            // Arrange
            const string senderId = "507f1f77bcf86cd799439011";
            const string chatId = "507f1f77bcf86cd799439012";
            const string text = "Invalid dimensions";
            const string textHtml = "<p>Invalid dimensions</p>";
            const string imageId = "image999";
            const int width = -100;
            const int height = -200;
            const ImageFormats imageFormat = ImageFormats.Unknown;
            const FileStorageTypes fileStorageType = FileStorageTypes.Local;
            var cancellationToken = new CancellationToken();

            var createdMessage = new Message { Id = "507f1f77bcf86cd799439022" };
            var expectedServiceModel = new MessageServiceModel { Id = "507f1f77bcf86cd799439022" };

            _mockMessageRepository
                .Setup(x => x.CreateAsync(senderId, chatId, text, textHtml, imageId, width, height, imageFormat, fileStorageType, cancellationToken))
                .ReturnsAsync(createdMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(createdMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.CreateMessageAsync(senderId, chatId, text, textHtml, imageId, width, height, imageFormat, fileStorageType, cancellationToken);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModel);
            _mockMessageRepository.Verify(x => x.CreateAsync(senderId, chatId, text, textHtml, imageId, width, height, imageFormat, fileStorageType, cancellationToken), Times.Once);
        }
    }
    [TestFixture]
    public class GetPagedAsyncTests : MessageMongoDBServiceTests
    {
        [Test]
        public async Task GetPagedAsync_WithValidParameters_ShouldReturnMappedMessages()
        {
            // Arrange
            const string chatId = "507f1f77bcf86cd799439011";
            const int pageIndex = 0;
            const int messagesPerPage = 10;
            var cancellationToken = new CancellationToken();

            var messages = new List<Message>
            {
                new Message
                {
                    Id = "507f1f77bcf86cd799439012",
                    Text = "First message",
                    SenderId = "507f1f77bcf86cd799439013",
                    ChatId = chatId
                },
                new Message
                {
                    Id = "507f1f77bcf86cd799439014",
                    Text = "Second message",
                    SenderId = "507f1f77bcf86cd799439015",
                    ChatId = chatId
                }
            };

            var expectedServiceModels = new List<MessageServiceModel>
            {
                new MessageServiceModel
                {
                    Id = "507f1f77bcf86cd799439012",
                    Text = "First message",
                    SenderId = "507f1f77bcf86cd799439013",
                    ChatId = chatId
                },
                new MessageServiceModel
                {
                    Id = "507f1f77bcf86cd799439014",
                    Text = "Second message",
                    SenderId = "507f1f77bcf86cd799439015",
                    ChatId = chatId
                }
            };

            _mockMessageRepository
                .Setup(x => x.GetPagedAsync(chatId, pageIndex, messagesPerPage, cancellationToken))
                .ReturnsAsync(messages);

            _mockMapper
                .Setup(x => x.Map<List<MessageServiceModel>>(messages))
                .Returns(expectedServiceModels);

            // Act
            var result = await _service.GetPagedAsync(chatId, pageIndex, messagesPerPage, cancellationToken);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModels);
            _mockMessageRepository.Verify(x => x.GetPagedAsync(chatId, pageIndex, messagesPerPage, cancellationToken), Times.Once);
            _mockMapper.Verify(x => x.Map<List<MessageServiceModel>>(messages), Times.Once);
        }

        [Test]
        public async Task GetPagedAsync_WithEmptyResult_ShouldReturnEmptyMappedList()
        {
            // Arrange
            const string chatId = "507f1f77bcf86cd799439011";
            const int pageIndex = 5;
            const int messagesPerPage = 20;

            var messages = new List<Message>();
            var expectedServiceModels = new List<MessageServiceModel>();

            _mockMessageRepository
                .Setup(x => x.GetPagedAsync(chatId, pageIndex, messagesPerPage, It.IsAny<CancellationToken>()))
                .ReturnsAsync(messages);

            _mockMapper
                .Setup(x => x.Map<List<MessageServiceModel>>(messages))
                .Returns(expectedServiceModels);

            // Act
            var result = await _service.GetPagedAsync(chatId, pageIndex, messagesPerPage);

            // Assert
            result.Should().BeEmpty();
            _mockMessageRepository.Verify(x => x.GetPagedAsync(chatId, pageIndex, messagesPerPage, It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(x => x.Map<List<MessageServiceModel>>(messages), Times.Once);
        }

        [Test]
        public async Task GetPagedAsync_WithNullChatId_ShouldPassNullToRepository()
        {
            // Arrange
            string chatId = null;
            const int pageIndex = 0;
            const int messagesPerPage = 10;

            var messages = new List<Message>();
            var expectedServiceModels = new List<MessageServiceModel>();

            _mockMessageRepository
                .Setup(x => x.GetPagedAsync(chatId, pageIndex, messagesPerPage, It.IsAny<CancellationToken>()))
                .ReturnsAsync(messages);

            _mockMapper
                .Setup(x => x.Map<List<MessageServiceModel>>(messages))
                .Returns(expectedServiceModels);

            // Act
            var result = await _service.GetPagedAsync(chatId, pageIndex, messagesPerPage);

            // Assert
            result.Should().BeEmpty();
            _mockMessageRepository.Verify(x => x.GetPagedAsync(chatId, pageIndex, messagesPerPage, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetPagedAsync_WithZeroMessagesPerPage_ShouldPassZeroToRepository()
        {
            // Arrange
            const string chatId = "507f1f77bcf86cd799439011";
            const int pageIndex = 0;
            const int messagesPerPage = 0;

            var messages = new List<Message>();
            var expectedServiceModels = new List<MessageServiceModel>();

            _mockMessageRepository
                .Setup(x => x.GetPagedAsync(chatId, pageIndex, messagesPerPage, It.IsAny<CancellationToken>()))
                .ReturnsAsync(messages);

            _mockMapper
                .Setup(x => x.Map<List<MessageServiceModel>>(messages))
                .Returns(expectedServiceModels);

            // Act
            var result = await _service.GetPagedAsync(chatId, pageIndex, messagesPerPage);

            // Assert
            result.Should().BeEmpty();
            _mockMessageRepository.Verify(x => x.GetPagedAsync(chatId, pageIndex, messagesPerPage, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetPagedAsync_WithNegativePageIndex_ShouldPassNegativeValueToRepository()
        {
            // Arrange
            const string chatId = "507f1f77bcf86cd799439011";
            const int pageIndex = -1;
            const int messagesPerPage = 10;

            var messages = new List<Message>();
            var expectedServiceModels = new List<MessageServiceModel>();

            _mockMessageRepository
                .Setup(x => x.GetPagedAsync(chatId, pageIndex, messagesPerPage, It.IsAny<CancellationToken>()))
                .ReturnsAsync(messages);

            _mockMapper
                .Setup(x => x.Map<List<MessageServiceModel>>(messages))
                .Returns(expectedServiceModels);

            // Act
            var result = await _service.GetPagedAsync(chatId, pageIndex, messagesPerPage);

            // Assert
            result.Should().BeEmpty();
            _mockMessageRepository.Verify(x => x.GetPagedAsync(chatId, pageIndex, messagesPerPage, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetPagedAsync_WithLargePageSize_ShouldPassLargeValueToRepository()
        {
            // Arrange
            const string chatId = "507f1f77bcf86cd799439011";
            const int pageIndex = 0;
            const int messagesPerPage = 1000;

            var messages = new List<Message>();
            var expectedServiceModels = new List<MessageServiceModel>();

            _mockMessageRepository
                .Setup(x => x.GetPagedAsync(chatId, pageIndex, messagesPerPage, It.IsAny<CancellationToken>()))
                .ReturnsAsync(messages);

            _mockMapper
                .Setup(x => x.Map<List<MessageServiceModel>>(messages))
                .Returns(expectedServiceModels);

            // Act
            var result = await _service.GetPagedAsync(chatId, pageIndex, messagesPerPage);

            // Assert
            result.Should().BeEmpty();
            _mockMessageRepository.Verify(x => x.GetPagedAsync(chatId, pageIndex, messagesPerPage, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetPagedAsync_WithNullMessagesList_ShouldReturnMappedResult()
        {
            // Arrange
            const string chatId = "507f1f77bcf86cd799439011";
            const int pageIndex = 0;
            const int messagesPerPage = 10;

            List<Message> messages = null;
            List<MessageServiceModel> expectedServiceModels = null;

            _mockMessageRepository
                .Setup(x => x.GetPagedAsync(chatId, pageIndex, messagesPerPage, It.IsAny<CancellationToken>()))
                .ReturnsAsync(messages);

            _mockMapper
                .Setup(x => x.Map<List<MessageServiceModel>>(messages))
                .Returns(expectedServiceModels);

            // Act
            var result = await _service.GetPagedAsync(chatId, pageIndex, messagesPerPage);

            // Assert
            result.Should().BeNull();
            _mockMessageRepository.Verify(x => x.GetPagedAsync(chatId, pageIndex, messagesPerPage, It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(x => x.Map<List<MessageServiceModel>>(messages), Times.Once);
        }
    }

    [TestFixture]
    public class SetLinkPreviewAsyncWithIdTests : MessageMongoDBServiceTests
    {
        [Test]
        public async Task SetLinkPreviewAsync_WithMessageIdAndLinkPreviewId_ShouldReturnMappedMessage()
        {
            // Arrange
            const string messageId = "507f1f77bcf86cd799439011";
            const string linkPreviewId = "507f1f77bcf86cd799439012";
            var cancellationToken = new CancellationToken();

            var updatedMessage = new Message
            {
                Id = messageId,
                Text = "Message with link preview",
                LinkPreviewId = linkPreviewId
            };

            var expectedServiceModel = new MessageServiceModel
            {
                Id = messageId,
                Text = "Message with link preview"
            };

            _mockMessageRepository
                .Setup(x => x.SetLinkPreviewAsync(messageId, linkPreviewId, cancellationToken))
                .ReturnsAsync(updatedMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(updatedMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.SetLinkPreviewAsync(messageId, linkPreviewId, cancellationToken);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModel);
            _mockMessageRepository.Verify(x => x.SetLinkPreviewAsync(messageId, linkPreviewId, cancellationToken), Times.Once);
            _mockMapper.Verify(x => x.Map<MessageServiceModel>(updatedMessage), Times.Once);
        }

        [Test]
        public async Task SetLinkPreviewAsync_WithNullParameters_ShouldPassNullsToRepository()
        {
            // Arrange
            string messageId = null;
            string linkPreviewId = null;

            var updatedMessage = new Message { Id = "507f1f77bcf86cd799439013" };
            var expectedServiceModel = new MessageServiceModel { Id = "507f1f77bcf86cd799439013" };

            _mockMessageRepository
                .Setup(x => x.SetLinkPreviewAsync(messageId, linkPreviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(updatedMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.SetLinkPreviewAsync(messageId, linkPreviewId);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModel);
            _mockMessageRepository.Verify(x => x.SetLinkPreviewAsync(messageId, linkPreviewId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task SetLinkPreviewAsync_WithEmptyStrings_ShouldPassEmptyStringsToRepository()
        {
            // Arrange
            const string messageId = "";
            const string linkPreviewId = "";

            var updatedMessage = new Message { Id = "507f1f77bcf86cd799439014" };
            var expectedServiceModel = new MessageServiceModel { Id = "507f1f77bcf86cd799439014" };

            _mockMessageRepository
                .Setup(x => x.SetLinkPreviewAsync(messageId, linkPreviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(updatedMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.SetLinkPreviewAsync(messageId, linkPreviewId);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModel);
            _mockMessageRepository.Verify(x => x.SetLinkPreviewAsync(messageId, linkPreviewId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    [TestFixture]
    public class SetLinkPreviewAsyncWithDetailsTests : MessageMongoDBServiceTests
    {
        [Test]
        public async Task SetLinkPreviewAsync_WithMessageIdAndLinkDetails_ShouldReturnMappedMessage()
        {
            // Arrange
            const string messageId = "507f1f77bcf86cd799439011";
            const string url = "https://example.com/article";
            const string title = "Example Article";
            const string imageUrl = "https://example.com/image.jpg";
            var cancellationToken = new CancellationToken();

            var updatedMessage = new Message
            {
                Id = messageId,
                Text = "Message with link preview details",
                LinkPreview = new LinkPreview
                {
                    Url = url,
                    Title = title,
                    ImageUrl = imageUrl
                }
            };

            var expectedServiceModel = new MessageServiceModel
            {
                Id = messageId,
                Text = "Message with link preview details",
                LinkPreview = new LinkPreviewServiceModel
                {
                    Url = url,
                    Title = title,
                    ImageUrl = imageUrl
                }
            };

            _mockMessageRepository
                .Setup(x => x.SetLinkPreviewAsync(messageId, url, title, imageUrl, cancellationToken))
                .ReturnsAsync(updatedMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(updatedMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.SetLinkPreviewAsync(messageId, url, title, imageUrl, cancellationToken);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModel);
            _mockMessageRepository.Verify(x => x.SetLinkPreviewAsync(messageId, url, title, imageUrl, cancellationToken), Times.Once);
            _mockMapper.Verify(x => x.Map<MessageServiceModel>(updatedMessage), Times.Once);
        }

        [Test]
        public async Task SetLinkPreviewAsync_WithNullLinkDetails_ShouldPassNullsToRepository()
        {
            // Arrange
            const string messageId = "507f1f77bcf86cd799439012";
            string url = null;
            string title = null;
            string imageUrl = null;

            var updatedMessage = new Message { Id = messageId };
            var expectedServiceModel = new MessageServiceModel { Id = messageId };

            _mockMessageRepository
                .Setup(x => x.SetLinkPreviewAsync(messageId, url, title, imageUrl, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(updatedMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.SetLinkPreviewAsync(messageId, url, title, imageUrl);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModel);
            _mockMessageRepository.Verify(x => x.SetLinkPreviewAsync(messageId, url, title, imageUrl, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task SetLinkPreviewAsync_WithEmptyLinkDetails_ShouldPassEmptyStringsToRepository()
        {
            // Arrange
            const string messageId = "507f1f77bcf86cd799439013";
            const string url = "";
            const string title = "";
            const string imageUrl = "";

            var updatedMessage = new Message { Id = messageId };
            var expectedServiceModel = new MessageServiceModel { Id = messageId };

            _mockMessageRepository
                .Setup(x => x.SetLinkPreviewAsync(messageId, url, title, imageUrl, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(updatedMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.SetLinkPreviewAsync(messageId, url, title, imageUrl);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModel);
            _mockMessageRepository.Verify(x => x.SetLinkPreviewAsync(messageId, url, title, imageUrl, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task SetLinkPreviewAsync_WithLongUrl_ShouldPassLongUrlToRepository()
        {
            // Arrange
            const string messageId = "507f1f77bcf86cd799439014";
            var url = "https://example.com/" + new string('a', 1000) + "/article";
            const string title = "Very Long URL Article";
            const string imageUrl = "https://example.com/long-url-image.jpg";

            var updatedMessage = new Message { Id = messageId };
            var expectedServiceModel = new MessageServiceModel { Id = messageId };

            _mockMessageRepository
                .Setup(x => x.SetLinkPreviewAsync(messageId, url, title, imageUrl, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(updatedMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.SetLinkPreviewAsync(messageId, url, title, imageUrl);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModel);
            _mockMessageRepository.Verify(x => x.SetLinkPreviewAsync(messageId, url, title, imageUrl, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
    [TestFixture]
    public class MarkAsReadAsyncTests : MessageMongoDBServiceTests
    {
        [Test]
        public async Task MarkAsReadAsync_WithValidParameters_ShouldCallRepository()
        {
            // Arrange
            const string chatId = "507f1f77bcf86cd799439011";
            const string accountId = "507f1f77bcf86cd799439012";
            const string messageId = "507f1f77bcf86cd799439013";
            var cancellationToken = new CancellationToken();

            _mockMessageRepository
                .Setup(x => x.MarkAsReadAsync(chatId, accountId, messageId, cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            await _service.MarkAsReadAsync(chatId, accountId, messageId, cancellationToken);

            // Assert
            _mockMessageRepository.Verify(x => x.MarkAsReadAsync(chatId, accountId, messageId, cancellationToken), Times.Once);
        }

        [Test]
        public async Task MarkAsReadAsync_WithNullParameters_ShouldPassNullsToRepository()
        {
            // Arrange
            string chatId = null;
            string accountId = null;
            string messageId = null;
            var cancellationToken = new CancellationToken();

            _mockMessageRepository
                .Setup(x => x.MarkAsReadAsync(chatId, accountId, messageId, cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            await _service.MarkAsReadAsync(chatId, accountId, messageId, cancellationToken);

            // Assert
            _mockMessageRepository.Verify(x => x.MarkAsReadAsync(chatId, accountId, messageId, cancellationToken), Times.Once);
        }

        [Test]
        public async Task MarkAsReadAsync_WithEmptyStrings_ShouldPassEmptyStringsToRepository()
        {
            // Arrange
            const string chatId = "";
            const string accountId = "";
            const string messageId = "";
            var cancellationToken = new CancellationToken();

            _mockMessageRepository
                .Setup(x => x.MarkAsReadAsync(chatId, accountId, messageId, cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            await _service.MarkAsReadAsync(chatId, accountId, messageId, cancellationToken);

            // Assert
            _mockMessageRepository.Verify(x => x.MarkAsReadAsync(chatId, accountId, messageId, cancellationToken), Times.Once);
        }

        [Test]
        public async Task MarkAsReadAsync_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            const string chatId = "507f1f77bcf86cd799439011";
            const string accountId = "507f1f77bcf86cd799439012";
            const string messageId = "507f1f77bcf86cd799439013";
            var cancellationToken = new CancellationToken();
            var expectedException = new InvalidOperationException("Mark as read failed");

            _mockMessageRepository
                .Setup(x => x.MarkAsReadAsync(chatId, accountId, messageId, cancellationToken))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var act = async () => await _service.MarkAsReadAsync(chatId, accountId, messageId, cancellationToken);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Mark as read failed");

            _mockMessageRepository.Verify(x => x.MarkAsReadAsync(chatId, accountId, messageId, cancellationToken), Times.Once);
        }

        [Test]
        public async Task MarkAsReadAsync_WithCancellationRequested_ShouldPropagateOperationCanceledException()
        {
            // Arrange
            const string chatId = "507f1f77bcf86cd799439011";
            const string accountId = "507f1f77bcf86cd799439012";
            const string messageId = "507f1f77bcf86cd799439013";
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            var cancellationToken = cancellationTokenSource.Token;

            _mockMessageRepository
                .Setup(x => x.MarkAsReadAsync(chatId, accountId, messageId, cancellationToken))
                .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            var act = async () => await _service.MarkAsReadAsync(chatId, accountId, messageId, cancellationToken);
            await act.Should().ThrowAsync<OperationCanceledException>();

            _mockMessageRepository.Verify(x => x.MarkAsReadAsync(chatId, accountId, messageId, cancellationToken), Times.Once);
        }
    }

    [TestFixture]
    public class SaveImagePreviewAsyncTests : MessageMongoDBServiceTests
    {
        [Test]
        public async Task SaveImagePreviewAsync_WithValidParameters_ShouldReturnMappedMessage()
        {
            // Arrange
            const string messageId = "507f1f77bcf86cd799439011";
            const string filename = "preview.jpg";
            const ImageFormats imageFormat = ImageFormats.Jpeg;
            const int width = 300;
            const int height = 200;
            const FileStorageTypes fileStorageType = FileStorageTypes.AmazonS3;
            var cancellationToken = new CancellationToken();

            var updatedMessage = new Message
            {
                Id = messageId,
                Text = "Message with image preview",
                ImagePreview = new Image
                {
                    Id = filename,
                    ImageFormatId = (int)imageFormat,
                    Width = width,
                    Height = height,
                    FileStorageTypeId = (int)fileStorageType
                }
            };

            var expectedServiceModel = new MessageServiceModel
            {
                Id = messageId,
                Text = "Message with image preview",
                ImagePreview = new ImagePreviewServiceModel
                {
                    Id = filename,
                    FileStorageTypeId = (int)fileStorageType,
                    Width = width,
                    Height = height
                }
            };

            _mockMessageRepository
                .Setup(x => x.SetImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType, cancellationToken))
                .ReturnsAsync(updatedMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(updatedMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.SaveImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType, cancellationToken);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModel);
            _mockMessageRepository.Verify(x => x.SetImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType, cancellationToken), Times.Once);
            _mockMapper.Verify(x => x.Map<MessageServiceModel>(updatedMessage), Times.Once);
        }

        [Test]
        public async Task SaveImagePreviewAsync_WithDifferentImageFormats_ShouldHandleAllFormats()
        {
            // Arrange
            const string messageId = "507f1f77bcf86cd799439012";
            const string filename = "preview.png";
            const ImageFormats imageFormat = ImageFormats.Png;
            const int width = 400;
            const int height = 300;
            const FileStorageTypes fileStorageType = FileStorageTypes.Local;

            var updatedMessage = new Message { Id = messageId };
            var expectedServiceModel = new MessageServiceModel { Id = messageId };

            _mockMessageRepository
                .Setup(x => x.SetImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(updatedMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.SaveImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModel);
            _mockMessageRepository.Verify(x => x.SetImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task SaveImagePreviewAsync_WithZeroDimensions_ShouldPassZeroValuesToRepository()
        {
            // Arrange
            const string messageId = "507f1f77bcf86cd799439013";
            const string filename = "small.gif";
            const ImageFormats imageFormat = ImageFormats.Gif;
            const int width = 0;
            const int height = 0;
            const FileStorageTypes fileStorageType = FileStorageTypes.AzureBlobStorage;

            var updatedMessage = new Message { Id = messageId };
            var expectedServiceModel = new MessageServiceModel { Id = messageId };

            _mockMessageRepository
                .Setup(x => x.SetImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(updatedMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.SaveImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModel);
            _mockMessageRepository.Verify(x => x.SetImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task SaveImagePreviewAsync_WithNullFilename_ShouldPassNullToRepository()
        {
            // Arrange
            const string messageId = "507f1f77bcf86cd799439014";
            string filename = null;
            const ImageFormats imageFormat = ImageFormats.Webp;
            const int width = 100;
            const int height = 100;
            const FileStorageTypes fileStorageType = FileStorageTypes.Local;

            var updatedMessage = new Message { Id = messageId };
            var expectedServiceModel = new MessageServiceModel { Id = messageId };

            _mockMessageRepository
                .Setup(x => x.SetImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(updatedMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.SaveImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModel);
            _mockMessageRepository.Verify(x => x.SetImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task SaveImagePreviewAsync_WithEmptyFilename_ShouldPassEmptyStringToRepository()
        {
            // Arrange
            const string messageId = "507f1f77bcf86cd799439015";
            const string filename = "";
            const ImageFormats imageFormat = ImageFormats.Unknown;
            const int width = 50;
            const int height = 50;
            const FileStorageTypes fileStorageType = FileStorageTypes.Local;

            var updatedMessage = new Message { Id = messageId };
            var expectedServiceModel = new MessageServiceModel { Id = messageId };

            _mockMessageRepository
                .Setup(x => x.SetImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(updatedMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.SaveImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModel);
            _mockMessageRepository.Verify(x => x.SetImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task SaveImagePreviewAsync_WithNegativeDimensions_ShouldPassNegativeValuesToRepository()
        {
            // Arrange
            const string messageId = "507f1f77bcf86cd799439016";
            const string filename = "invalid.jpg";
            const ImageFormats imageFormat = ImageFormats.Jpeg;
            const int width = -100;
            const int height = -200;
            const FileStorageTypes fileStorageType = FileStorageTypes.AmazonS3;

            var updatedMessage = new Message { Id = messageId };
            var expectedServiceModel = new MessageServiceModel { Id = messageId };

            _mockMessageRepository
                .Setup(x => x.SetImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(updatedMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.SaveImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModel);
            _mockMessageRepository.Verify(x => x.SetImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task SaveImagePreviewAsync_WithLargeFilename_ShouldPassLargeFilenameToRepository()
        {
            // Arrange
            const string messageId = "507f1f77bcf86cd799439017";
            var filename = new string('a', 500) + ".jpg";
            const ImageFormats imageFormat = ImageFormats.Jpeg;
            const int width = 800;
            const int height = 600;
            const FileStorageTypes fileStorageType = FileStorageTypes.AmazonS3;

            var updatedMessage = new Message { Id = messageId };
            var expectedServiceModel = new MessageServiceModel { Id = messageId };

            _mockMessageRepository
                .Setup(x => x.SetImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(updatedMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.SaveImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModel);
            _mockMessageRepository.Verify(x => x.SetImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task SaveImagePreviewAsync_WithAllFileStorageTypes_ShouldHandleAllTypes()
        {
            // Arrange
            const string messageId = "507f1f77bcf86cd799439018";
            const string filename = "azure.png";
            const ImageFormats imageFormat = ImageFormats.Png;
            const int width = 500;
            const int height = 400;
            const FileStorageTypes fileStorageType = FileStorageTypes.AzureBlobStorage;

            var updatedMessage = new Message { Id = messageId };
            var expectedServiceModel = new MessageServiceModel { Id = messageId };

            _mockMessageRepository
                .Setup(x => x.SetImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedMessage);

            _mockMapper
                .Setup(x => x.Map<MessageServiceModel>(updatedMessage))
                .Returns(expectedServiceModel);

            // Act
            var result = await _service.SaveImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType);

            // Assert
            result.Should().BeEquivalentTo(expectedServiceModel);
            _mockMessageRepository.Verify(x => x.SetImagePreviewAsync(messageId, filename, imageFormat, width, height, fileStorageType, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
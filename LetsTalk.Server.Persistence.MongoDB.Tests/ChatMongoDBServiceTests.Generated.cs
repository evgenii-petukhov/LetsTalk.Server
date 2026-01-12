using FluentAssertions;
using LetsTalk.Server.Persistence.MongoDB.Models;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions.Models;
using LetsTalk.Server.Persistence.MongoDB.Services;
using Moq;
using NUnit.Framework;

namespace LetsTalk.Server.Persistence.MongoDB.Tests;

[TestFixture]
public class ChatMongoDBServiceGeneratedTests
{
    private Mock<IChatRepository> _mockChatRepository;
    private ChatMongoDBService _service;

    [SetUp]
    public void SetUp()
    {
        _mockChatRepository = new Mock<IChatRepository>();
        _service = new ChatMongoDBService(_mockChatRepository.Object);
    }

    [TestFixture]
    public class GetChatMemberAccountIdsAsyncTests : ChatMongoDBServiceGeneratedTests
    {
        [Test]
        public async Task GetChatMemberAccountIdsAsync_WithValidChatId_ShouldReturnAccountIds()
        {
            // Arrange
            const string chatId = "507f1f77bcf86cd799439011";
            var expectedAccountIds = new List<string> { "507f1f77bcf86cd799439012", "507f1f77bcf86cd799439013" };

            _mockChatRepository
                .Setup(x => x.GetChatMemberAccountIdsAsync(chatId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAccountIds);

            // Act
            var result = await _service.GetChatMemberAccountIdsAsync(chatId);

            // Assert
            result.Should().BeEquivalentTo(expectedAccountIds);
            _mockChatRepository.Verify(x => x.GetChatMemberAccountIdsAsync(chatId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetChatMemberAccountIdsAsync_WithEmptyResult_ShouldReturnEmptyList()
        {
            // Arrange
            const string chatId = "507f1f77bcf86cd799439011";
            var expectedAccountIds = new List<string>();

            _mockChatRepository
                .Setup(x => x.GetChatMemberAccountIdsAsync(chatId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAccountIds);

            // Act
            var result = await _service.GetChatMemberAccountIdsAsync(chatId);

            // Assert
            result.Should().BeEmpty();
            _mockChatRepository.Verify(x => x.GetChatMemberAccountIdsAsync(chatId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetChatMemberAccountIdsAsync_WithCancellationToken_ShouldPassTokenToRepository()
        {
            // Arrange
            const string chatId = "507f1f77bcf86cd799439011";
            var cancellationToken = new CancellationToken();
            var expectedAccountIds = new List<string> { "507f1f77bcf86cd799439012" };

            _mockChatRepository
                .Setup(x => x.GetChatMemberAccountIdsAsync(chatId, cancellationToken))
                .ReturnsAsync(expectedAccountIds);

            // Act
            await _service.GetChatMemberAccountIdsAsync(chatId, cancellationToken);

            // Assert
            _mockChatRepository.Verify(x => x.GetChatMemberAccountIdsAsync(chatId, cancellationToken), Times.Once);
        }

        [Test]
        public async Task GetChatMemberAccountIdsAsync_WithNullChatId_ShouldPassNullToRepository()
        {
            // Arrange
            string chatId = null;
            var expectedAccountIds = new List<string>();

            _mockChatRepository
                .Setup(x => x.GetChatMemberAccountIdsAsync(chatId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAccountIds);

            // Act
            var result = await _service.GetChatMemberAccountIdsAsync(chatId);

            // Assert
            result.Should().BeEmpty();
            _mockChatRepository.Verify(x => x.GetChatMemberAccountIdsAsync(chatId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    [TestFixture]
    public class IsChatIdValidAsyncTests : ChatMongoDBServiceGeneratedTests
    {
        [Test]
        public async Task IsChatIdValidAsync_WithValidChatId_ShouldReturnTrue()
        {
            // Arrange
            const string chatId = "507f1f77bcf86cd799439011";

            _mockChatRepository
                .Setup(x => x.IsChatIdValidAsync(chatId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.IsChatIdValidAsync(chatId);

            // Assert
            result.Should().BeTrue();
            _mockChatRepository.Verify(x => x.IsChatIdValidAsync(chatId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task IsChatIdValidAsync_WithInvalidChatId_ShouldReturnFalse()
        {
            // Arrange
            const string chatId = "invalid-chat-id";

            _mockChatRepository
                .Setup(x => x.IsChatIdValidAsync(chatId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.IsChatIdValidAsync(chatId);

            // Assert
            result.Should().BeFalse();
            _mockChatRepository.Verify(x => x.IsChatIdValidAsync(chatId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task IsChatIdValidAsync_WithCancellationToken_ShouldPassTokenToRepository()
        {
            // Arrange
            const string chatId = "507f1f77bcf86cd799439011";
            var cancellationToken = new CancellationToken();

            _mockChatRepository
                .Setup(x => x.IsChatIdValidAsync(chatId, cancellationToken))
                .ReturnsAsync(true);

            // Act
            await _service.IsChatIdValidAsync(chatId, cancellationToken);

            // Assert
            _mockChatRepository.Verify(x => x.IsChatIdValidAsync(chatId, cancellationToken), Times.Once);
        }

        [Test]
        public async Task IsChatIdValidAsync_WithNullChatId_ShouldPassNullToRepository()
        {
            // Arrange
            string chatId = null;

            _mockChatRepository
                .Setup(x => x.IsChatIdValidAsync(chatId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.IsChatIdValidAsync(chatId);

            // Assert
            result.Should().BeFalse();
            _mockChatRepository.Verify(x => x.IsChatIdValidAsync(chatId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task IsChatIdValidAsync_WithEmptyString_ShouldPassEmptyStringToRepository()
        {
            // Arrange
            const string chatId = "";

            _mockChatRepository
                .Setup(x => x.IsChatIdValidAsync(chatId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.IsChatIdValidAsync(chatId);

            // Assert
            result.Should().BeFalse();
            _mockChatRepository.Verify(x => x.IsChatIdValidAsync(chatId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    [TestFixture]
    public class CreateIndividualChatAsyncTests : ChatMongoDBServiceGeneratedTests
    {
        [Test]
        public async Task CreateIndividualChatAsync_WhenChatExists_ShouldReturnExistingChatId()
        {
            // Arrange
            var accountIds = new List<string> { "507f1f77bcf86cd799439012", "507f1f77bcf86cd799439013" };
            var existingChat = new Chat
            {
                Id = "507f1f77bcf86cd799439011",
                AccountIds = accountIds,
                IsIndividual = true
            };

            _mockChatRepository
                .Setup(x => x.GetIndividualChatByAccountIdsAsync(accountIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingChat);

            // Act
            var result = await _service.CreateIndividualChatAsync(accountIds);

            // Assert
            result.Should().Be(existingChat.Id);
            _mockChatRepository.Verify(x => x.GetIndividualChatByAccountIdsAsync(accountIds, It.IsAny<CancellationToken>()), Times.Once);
            _mockChatRepository.Verify(x => x.CreateIndividualChatAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task CreateIndividualChatAsync_WhenChatDoesNotExist_ShouldCreateNewChatAndReturnId()
        {
            // Arrange
            var accountIds = new List<string> { "507f1f77bcf86cd799439012", "507f1f77bcf86cd799439013" };
            var newChat = new Chat
            {
                Id = "507f1f77bcf86cd799439014",
                AccountIds = accountIds,
                IsIndividual = true
            };

            _mockChatRepository
                .Setup(x => x.GetIndividualChatByAccountIdsAsync(accountIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Chat)null);

            _mockChatRepository
                .Setup(x => x.CreateIndividualChatAsync(accountIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(newChat);

            // Act
            var result = await _service.CreateIndividualChatAsync(accountIds);

            // Assert
            result.Should().Be(newChat.Id);
            _mockChatRepository.Verify(x => x.GetIndividualChatByAccountIdsAsync(accountIds, It.IsAny<CancellationToken>()), Times.Once);
            _mockChatRepository.Verify(x => x.CreateIndividualChatAsync(accountIds, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task CreateIndividualChatAsync_WithCancellationToken_ShouldPassTokenToAllMethods()
        {
            // Arrange
            var accountIds = new List<string> { "507f1f77bcf86cd799439012", "507f1f77bcf86cd799439013" };
            var cancellationToken = new CancellationToken();
            var existingChat = new Chat
            {
                Id = "507f1f77bcf86cd799439011",
                AccountIds = accountIds,
                IsIndividual = true
            };

            _mockChatRepository
                .Setup(x => x.GetIndividualChatByAccountIdsAsync(accountIds, cancellationToken))
                .ReturnsAsync(existingChat);

            // Act
            await _service.CreateIndividualChatAsync(accountIds, cancellationToken);

            // Assert
            _mockChatRepository.Verify(x => x.GetIndividualChatByAccountIdsAsync(accountIds, cancellationToken), Times.Once);
        }

        [Test]
        public async Task CreateIndividualChatAsync_WithEmptyAccountIds_ShouldPassEmptyListToRepository()
        {
            // Arrange
            var accountIds = new List<string>();
            var newChat = new Chat
            {
                Id = "507f1f77bcf86cd799439014",
                AccountIds = accountIds,
                IsIndividual = true
            };

            _mockChatRepository
                .Setup(x => x.GetIndividualChatByAccountIdsAsync(accountIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Chat)null);

            _mockChatRepository
                .Setup(x => x.CreateIndividualChatAsync(accountIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(newChat);

            // Act
            var result = await _service.CreateIndividualChatAsync(accountIds);

            // Assert
            result.Should().Be(newChat.Id);
            _mockChatRepository.Verify(x => x.GetIndividualChatByAccountIdsAsync(accountIds, It.IsAny<CancellationToken>()), Times.Once);
            _mockChatRepository.Verify(x => x.CreateIndividualChatAsync(accountIds, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task CreateIndividualChatAsync_WithSingleAccountId_ShouldCreateChat()
        {
            // Arrange
            var accountIds = new List<string> { "507f1f77bcf86cd799439012" };
            var newChat = new Chat
            {
                Id = "507f1f77bcf86cd799439014",
                AccountIds = accountIds,
                IsIndividual = true
            };

            _mockChatRepository
                .Setup(x => x.GetIndividualChatByAccountIdsAsync(accountIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Chat)null);

            _mockChatRepository
                .Setup(x => x.CreateIndividualChatAsync(accountIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(newChat);

            // Act
            var result = await _service.CreateIndividualChatAsync(accountIds);

            // Assert
            result.Should().Be(newChat.Id);
            _mockChatRepository.Verify(x => x.CreateIndividualChatAsync(accountIds, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task CreateIndividualChatAsync_WhenRepositoryReturnsNullForNewChat_ShouldThrowNullReferenceException()
        {
            // Arrange
            var accountIds = new List<string> { "507f1f77bcf86cd799439012", "507f1f77bcf86cd799439013" };

            _mockChatRepository
                .Setup(x => x.GetIndividualChatByAccountIdsAsync(accountIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Chat)null);

            _mockChatRepository
                .Setup(x => x.CreateIndividualChatAsync(accountIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Chat)null);

            // Act & Assert
            var act = async () => await _service.CreateIndividualChatAsync(accountIds);
            await act.Should().ThrowAsync<NullReferenceException>();
            
            _mockChatRepository.Verify(x => x.GetIndividualChatByAccountIdsAsync(accountIds, It.IsAny<CancellationToken>()), Times.Once);
            _mockChatRepository.Verify(x => x.CreateIndividualChatAsync(accountIds, It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    [TestFixture]
    public class GetAccountIdsInIndividualChatsAsyncTests : ChatMongoDBServiceGeneratedTests
    {
        [Test]
        public async Task GetAccountIdsInIndividualChatsAsync_WithValidAccountId_ShouldReturnAccountIds()
        {
            // Arrange
            const string accountId = "507f1f77bcf86cd799439012";
            var expectedAccountIds = new List<string> { "507f1f77bcf86cd799439013", "507f1f77bcf86cd799439014", "507f1f77bcf86cd799439015" };

            _mockChatRepository
                .Setup(x => x.GetAccountIdsInIndividualChatsAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAccountIds);

            // Act
            var result = await _service.GetAccountIdsInIndividualChatsAsync(accountId);

            // Assert
            result.Should().BeEquivalentTo(expectedAccountIds);
            _mockChatRepository.Verify(x => x.GetAccountIdsInIndividualChatsAsync(accountId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetAccountIdsInIndividualChatsAsync_WithEmptyResult_ShouldReturnEmptyList()
        {
            // Arrange
            const string accountId = "507f1f77bcf86cd799439012";
            var expectedAccountIds = new List<string>();

            _mockChatRepository
                .Setup(x => x.GetAccountIdsInIndividualChatsAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAccountIds);

            // Act
            var result = await _service.GetAccountIdsInIndividualChatsAsync(accountId);

            // Assert
            result.Should().BeEmpty();
            _mockChatRepository.Verify(x => x.GetAccountIdsInIndividualChatsAsync(accountId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetAccountIdsInIndividualChatsAsync_WithCancellationToken_ShouldPassTokenToRepository()
        {
            // Arrange
            const string accountId = "507f1f77bcf86cd799439012";
            var cancellationToken = new CancellationToken();
            var expectedAccountIds = new List<string> { "507f1f77bcf86cd799439013" };

            _mockChatRepository
                .Setup(x => x.GetAccountIdsInIndividualChatsAsync(accountId, cancellationToken))
                .ReturnsAsync(expectedAccountIds);

            // Act
            await _service.GetAccountIdsInIndividualChatsAsync(accountId, cancellationToken);

            // Assert
            _mockChatRepository.Verify(x => x.GetAccountIdsInIndividualChatsAsync(accountId, cancellationToken), Times.Once);
        }

        [Test]
        public async Task GetAccountIdsInIndividualChatsAsync_WithNullAccountId_ShouldPassNullToRepository()
        {
            // Arrange
            string accountId = null;
            var expectedAccountIds = new List<string>();

            _mockChatRepository
                .Setup(x => x.GetAccountIdsInIndividualChatsAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAccountIds);

            // Act
            var result = await _service.GetAccountIdsInIndividualChatsAsync(accountId);

            // Assert
            result.Should().BeEmpty();
            _mockChatRepository.Verify(x => x.GetAccountIdsInIndividualChatsAsync(accountId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetAccountIdsInIndividualChatsAsync_WithEmptyString_ShouldPassEmptyStringToRepository()
        {
            // Arrange
            const string accountId = "";
            var expectedAccountIds = new List<string>();

            _mockChatRepository
                .Setup(x => x.GetAccountIdsInIndividualChatsAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAccountIds);

            // Act
            var result = await _service.GetAccountIdsInIndividualChatsAsync(accountId);

            // Assert
            result.Should().BeEmpty();
            _mockChatRepository.Verify(x => x.GetAccountIdsInIndividualChatsAsync(accountId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    [TestFixture]
    public class IsAccountChatMemberAsyncTests : ChatMongoDBServiceGeneratedTests
    {
        [Test]
        public async Task IsAccountChatMemberAsync_WhenAccountIsMember_ShouldReturnTrue()
        {
            // Arrange
            const string chatId = "507f1f77bcf86cd799439011";
            const string accountId = "507f1f77bcf86cd799439012";

            _mockChatRepository
                .Setup(x => x.IsAccountChatMemberAsync(chatId, accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.IsAccountChatMemberAsync(chatId, accountId);

            // Assert
            result.Should().BeTrue();
            _mockChatRepository.Verify(x => x.IsAccountChatMemberAsync(chatId, accountId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task IsAccountChatMemberAsync_WhenAccountIsNotMember_ShouldReturnFalse()
        {
            // Arrange
            const string chatId = "507f1f77bcf86cd799439011";
            const string accountId = "507f1f77bcf86cd799439012";

            _mockChatRepository
                .Setup(x => x.IsAccountChatMemberAsync(chatId, accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.IsAccountChatMemberAsync(chatId, accountId);

            // Assert
            result.Should().BeFalse();
            _mockChatRepository.Verify(x => x.IsAccountChatMemberAsync(chatId, accountId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task IsAccountChatMemberAsync_WithCancellationToken_ShouldPassTokenToRepository()
        {
            // Arrange
            const string chatId = "507f1f77bcf86cd799439011";
            const string accountId = "507f1f77bcf86cd799439012";
            var cancellationToken = new CancellationToken();

            _mockChatRepository
                .Setup(x => x.IsAccountChatMemberAsync(chatId, accountId, cancellationToken))
                .ReturnsAsync(true);

            // Act
            await _service.IsAccountChatMemberAsync(chatId, accountId, cancellationToken);

            // Assert
            _mockChatRepository.Verify(x => x.IsAccountChatMemberAsync(chatId, accountId, cancellationToken), Times.Once);
        }

        [Test]
        public async Task IsAccountChatMemberAsync_WithNullChatId_ShouldPassNullToRepository()
        {
            // Arrange
            string chatId = null;
            const string accountId = "507f1f77bcf86cd799439012";

            _mockChatRepository
                .Setup(x => x.IsAccountChatMemberAsync(chatId, accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.IsAccountChatMemberAsync(chatId, accountId);

            // Assert
            result.Should().BeFalse();
            _mockChatRepository.Verify(x => x.IsAccountChatMemberAsync(chatId, accountId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task IsAccountChatMemberAsync_WithNullAccountId_ShouldPassNullToRepository()
        {
            // Arrange
            const string chatId = "507f1f77bcf86cd799439011";
            string accountId = null;

            _mockChatRepository
                .Setup(x => x.IsAccountChatMemberAsync(chatId, accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.IsAccountChatMemberAsync(chatId, accountId);

            // Assert
            result.Should().BeFalse();
            _mockChatRepository.Verify(x => x.IsAccountChatMemberAsync(chatId, accountId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task IsAccountChatMemberAsync_WithEmptyStrings_ShouldPassEmptyStringsToRepository()
        {
            // Arrange
            const string chatId = "";
            const string accountId = "";

            _mockChatRepository
                .Setup(x => x.IsAccountChatMemberAsync(chatId, accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.IsAccountChatMemberAsync(chatId, accountId);

            // Assert
            result.Should().BeFalse();
            _mockChatRepository.Verify(x => x.IsAccountChatMemberAsync(chatId, accountId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task IsAccountChatMemberAsync_WithValidObjectIds_ShouldReturnTrue()
        {
            // Arrange
            const string chatId = "507f1f77bcf86cd799439011";
            const string accountId = "507f1f77bcf86cd799439012";

            _mockChatRepository
                .Setup(x => x.IsAccountChatMemberAsync(chatId, accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.IsAccountChatMemberAsync(chatId, accountId);

            // Assert
            result.Should().BeTrue();
            _mockChatRepository.Verify(x => x.IsAccountChatMemberAsync(chatId, accountId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task IsAccountChatMemberAsync_WithInvalidObjectIds_ShouldPassToRepositoryAndReturnResult()
        {
            // Arrange
            const string chatId = "invalid-chat-id";
            const string accountId = "invalid-account-id";

            _mockChatRepository
                .Setup(x => x.IsAccountChatMemberAsync(chatId, accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.IsAccountChatMemberAsync(chatId, accountId);

            // Assert
            result.Should().BeFalse();
            _mockChatRepository.Verify(x => x.IsAccountChatMemberAsync(chatId, accountId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
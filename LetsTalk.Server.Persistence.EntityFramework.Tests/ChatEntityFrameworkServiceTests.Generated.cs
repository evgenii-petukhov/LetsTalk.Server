using AutoMapper;
using FluentAssertions;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using LetsTalk.Server.Persistence.EntityFramework.Services;
using Moq;
using NUnit.Framework;
using System.Globalization;
using System.Reflection;

namespace LetsTalk.Server.Persistence.EntityFramework.Tests;

[TestFixture]
public class ChatEntityFrameworkServiceGeneratedTests
{
    private Mock<IChatRepository> _mockChatRepository;
    private Mock<IChatMemberRepository> _mockChatMemberRepository;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IMapper> _mockMapper;
    private ChatEntityFrameworkService _service;

    [SetUp]
    public void SetUp()
    {
        _mockChatRepository = new Mock<IChatRepository>();
        _mockChatMemberRepository = new Mock<IChatMemberRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();

        _service = new ChatEntityFrameworkService(
            _mockChatRepository.Object,
            _mockChatMemberRepository.Object,
            _mockUnitOfWork.Object,
            _mockMapper.Object);
    }

    [TestFixture]
    public class GetChatMemberAccountIdsAsyncTests : ChatEntityFrameworkServiceGeneratedTests
    {
        [Test]
        public async Task GetChatMemberAccountIdsAsync_WithValidChatId_ShouldReturnAccountIds()
        {
            // Arrange
            const string chatId = "123";
            const int chatIdAsInt = 123;
            var accountIds = new List<int> { 1, 2, 3 };
            var expectedResult = new List<string> { "1", "2", "3" };

            _mockChatMemberRepository
                .Setup(x => x.GetChatMemberAccountIdsAsync(chatIdAsInt, It.IsAny<CancellationToken>()))
                .ReturnsAsync(accountIds);

            // Act
            var result = await _service.GetChatMemberAccountIdsAsync(chatId);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            _mockChatMemberRepository.Verify(x => x.GetChatMemberAccountIdsAsync(chatIdAsInt, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetChatMemberAccountIdsAsync_WithEmptyResult_ShouldReturnEmptyList()
        {
            // Arrange
            const string chatId = "456";
            const int chatIdAsInt = 456;
            var accountIds = new List<int>();

            _mockChatMemberRepository
                .Setup(x => x.GetChatMemberAccountIdsAsync(chatIdAsInt, It.IsAny<CancellationToken>()))
                .ReturnsAsync(accountIds);

            // Act
            var result = await _service.GetChatMemberAccountIdsAsync(chatId);

            // Assert
            result.Should().BeEmpty();
            _mockChatMemberRepository.Verify(x => x.GetChatMemberAccountIdsAsync(chatIdAsInt, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetChatMemberAccountIdsAsync_WithCancellationToken_ShouldPassTokenToRepository()
        {
            // Arrange
            const string chatId = "789";
            const int chatIdAsInt = 789;
            var cancellationToken = new CancellationToken();
            var accountIds = new List<int> { 10, 20 };

            _mockChatMemberRepository
                .Setup(x => x.GetChatMemberAccountIdsAsync(chatIdAsInt, cancellationToken))
                .ReturnsAsync(accountIds);

            // Act
            await _service.GetChatMemberAccountIdsAsync(chatId, cancellationToken);

            // Assert
            _mockChatMemberRepository.Verify(x => x.GetChatMemberAccountIdsAsync(chatIdAsInt, cancellationToken), Times.Once);
        }

        [Test]
        public void GetChatMemberAccountIdsAsync_WithInvalidChatId_ShouldThrowFormatException()
        {
            // Arrange
            const string invalidChatId = "invalid";

            // Act & Assert
            var act = async () => await _service.GetChatMemberAccountIdsAsync(invalidChatId);
            act.Should().ThrowAsync<FormatException>();
        }
    }

    [TestFixture]
    public class GetAccountIdsInIndividualChatsAsyncTests : ChatEntityFrameworkServiceGeneratedTests
    {
        [Test]
        public async Task GetAccountIdsInIndividualChatsAsync_WithValidAccountId_ShouldReturnAccountIds()
        {
            // Arrange
            const string accountId = "100";
            const int accountIdAsInt = 100;
            var accountIds = new List<int> { 200, 300, 400 };
            var expectedResult = new List<string> { "200", "300", "400" };

            _mockChatRepository
                .Setup(x => x.GetAccountIdsInIndividualChatsAsync(accountIdAsInt, It.IsAny<CancellationToken>()))
                .ReturnsAsync(accountIds);

            // Act
            var result = await _service.GetAccountIdsInIndividualChatsAsync(accountId);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            _mockChatRepository.Verify(x => x.GetAccountIdsInIndividualChatsAsync(accountIdAsInt, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetAccountIdsInIndividualChatsAsync_WithEmptyResult_ShouldReturnEmptyList()
        {
            // Arrange
            const string accountId = "500";
            const int accountIdAsInt = 500;
            var accountIds = new List<int>();

            _mockChatRepository
                .Setup(x => x.GetAccountIdsInIndividualChatsAsync(accountIdAsInt, It.IsAny<CancellationToken>()))
                .ReturnsAsync(accountIds);

            // Act
            var result = await _service.GetAccountIdsInIndividualChatsAsync(accountId);

            // Assert
            result.Should().BeEmpty();
            _mockChatRepository.Verify(x => x.GetAccountIdsInIndividualChatsAsync(accountIdAsInt, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetAccountIdsInIndividualChatsAsync_WithCancellationToken_ShouldPassTokenToRepository()
        {
            // Arrange
            const string accountId = "600";
            const int accountIdAsInt = 600;
            var cancellationToken = new CancellationToken();
            var accountIds = new List<int> { 700, 800 };

            _mockChatRepository
                .Setup(x => x.GetAccountIdsInIndividualChatsAsync(accountIdAsInt, cancellationToken))
                .ReturnsAsync(accountIds);

            // Act
            await _service.GetAccountIdsInIndividualChatsAsync(accountId, cancellationToken);

            // Assert
            _mockChatRepository.Verify(x => x.GetAccountIdsInIndividualChatsAsync(accountIdAsInt, cancellationToken), Times.Once);
        }

        [Test]
        public void GetAccountIdsInIndividualChatsAsync_WithInvalidAccountId_ShouldThrowFormatException()
        {
            // Arrange
            const string invalidAccountId = "not-a-number";

            // Act & Assert
            var act = async () => await _service.GetAccountIdsInIndividualChatsAsync(invalidAccountId);
            act.Should().ThrowAsync<FormatException>();
        }
    }

    [TestFixture]
    public class IsChatIdValidAsyncTests : ChatEntityFrameworkServiceGeneratedTests
    {
        [Test]
        public async Task IsChatIdValidAsync_WithValidChatId_ShouldReturnTrue()
        {
            // Arrange
            const string chatId = "123";
            const int chatIdAsInt = 123;

            _mockChatRepository
                .Setup(x => x.IsChatIdValidAsync(chatIdAsInt, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.IsChatIdValidAsync(chatId);

            // Assert
            result.Should().BeTrue();
            _mockChatRepository.Verify(x => x.IsChatIdValidAsync(chatIdAsInt, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task IsChatIdValidAsync_WithInvalidChatId_ShouldReturnFalse()
        {
            // Arrange
            const string chatId = "999";
            const int chatIdAsInt = 999;

            _mockChatRepository
                .Setup(x => x.IsChatIdValidAsync(chatIdAsInt, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.IsChatIdValidAsync(chatId);

            // Assert
            result.Should().BeFalse();
            _mockChatRepository.Verify(x => x.IsChatIdValidAsync(chatIdAsInt, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task IsChatIdValidAsync_WithCancellationToken_ShouldPassTokenToRepository()
        {
            // Arrange
            const string chatId = "456";
            const int chatIdAsInt = 456;
            var cancellationToken = new CancellationToken();

            _mockChatRepository
                .Setup(x => x.IsChatIdValidAsync(chatIdAsInt, cancellationToken))
                .ReturnsAsync(true);

            // Act
            await _service.IsChatIdValidAsync(chatId, cancellationToken);

            // Assert
            _mockChatRepository.Verify(x => x.IsChatIdValidAsync(chatIdAsInt, cancellationToken), Times.Once);
        }

        [Test]
        public void IsChatIdValidAsync_WithInvalidIdFormat_ShouldThrowFormatException()
        {
            // Arrange
            const string invalidChatId = "abc";

            // Act & Assert
            var act = async () => await _service.IsChatIdValidAsync(invalidChatId);
            act.Should().ThrowAsync<FormatException>();
        }
    }

    [TestFixture]
    public class CreateIndividualChatAsyncTests : ChatEntityFrameworkServiceGeneratedTests
    {
        [Test]
        public async Task CreateIndividualChatAsync_WhenChatDoesNotExist_ShouldCreateNewChatAndReturnId()
        {
            // Arrange
            var accountIds = new List<string> { "100", "200" };
            var accountIdsAsInt = new List<int> { 100, 200 };
            var newChat = new Chat(accountIdsAsInt);

            _mockChatRepository
                .Setup(x => x.GetIndividualChatByAccountIdsAsync(
                    It.Is<List<int>>(ids => ids.SequenceEqual(accountIdsAsInt)), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Chat)null);

            _mockChatRepository
                .Setup(x => x.CreateAsync(It.IsAny<Chat>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateIndividualChatAsync(accountIds);

            // Assert
            result.Should().Be("0"); // Default ID for new entity
            _mockChatRepository.Verify(x => x.GetIndividualChatByAccountIdsAsync(
                It.Is<List<int>>(ids => ids.SequenceEqual(accountIdsAsInt)), 
                It.IsAny<CancellationToken>()), Times.Once);
            _mockChatRepository.Verify(x => x.CreateAsync(It.IsAny<Chat>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task CreateIndividualChatAsync_WhenChatExists_ShouldReturnExistingChatId()
        {
            // Arrange
            var accountIds = new List<string> { "300", "400" };
            var accountIdsAsInt = new List<int> { 300, 400 };
            
            // Use reflection to create a chat with a specific ID
            var existingChat = new Chat(accountIdsAsInt);
            var idProperty = typeof(BaseEntity).GetProperty("Id");
            idProperty!.SetValue(existingChat, 5);

            _mockChatRepository
                .Setup(x => x.GetIndividualChatByAccountIdsAsync(
                    It.Is<List<int>>(ids => ids.SequenceEqual(accountIdsAsInt)), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingChat);

            // Act
            var result = await _service.CreateIndividualChatAsync(accountIds);

            // Assert
            result.Should().Be("5");
            _mockChatRepository.Verify(x => x.GetIndividualChatByAccountIdsAsync(
                It.Is<List<int>>(ids => ids.SequenceEqual(accountIdsAsInt)), 
                It.IsAny<CancellationToken>()), Times.Once);
            _mockChatRepository.Verify(x => x.CreateAsync(It.IsAny<Chat>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task CreateIndividualChatAsync_WithCancellationToken_ShouldPassTokenToAllMethods()
        {
            // Arrange
            var accountIds = new List<string> { "500", "600" };
            var accountIdsAsInt = new List<int> { 500, 600 };
            var cancellationToken = new CancellationToken();

            _mockChatRepository
                .Setup(x => x.GetIndividualChatByAccountIdsAsync(
                    It.Is<List<int>>(ids => ids.SequenceEqual(accountIdsAsInt)), 
                    cancellationToken))
                .ReturnsAsync((Chat)null);

            _mockChatRepository
                .Setup(x => x.CreateAsync(It.IsAny<Chat>(), cancellationToken))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(x => x.SaveAsync(cancellationToken))
                .Returns(Task.CompletedTask);

            // Act
            await _service.CreateIndividualChatAsync(accountIds, cancellationToken);

            // Assert
            _mockChatRepository.Verify(x => x.GetIndividualChatByAccountIdsAsync(
                It.Is<List<int>>(ids => ids.SequenceEqual(accountIdsAsInt)), 
                cancellationToken), Times.Once);
            _mockChatRepository.Verify(x => x.CreateAsync(It.IsAny<Chat>(), cancellationToken), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(cancellationToken), Times.Once);
        }

        [Test]
        public void CreateIndividualChatAsync_WithInvalidAccountIds_ShouldThrowFormatException()
        {
            // Arrange
            var invalidAccountIds = new List<string> { "invalid", "200" };

            // Act & Assert
            var act = async () => await _service.CreateIndividualChatAsync(invalidAccountIds);
            act.Should().ThrowAsync<FormatException>();
        }

        [Test]
        public async Task CreateIndividualChatAsync_WithEmptyAccountIds_ShouldPassEmptyListToRepository()
        {
            // Arrange
            var accountIds = new List<string>();
            var accountIdsAsInt = new List<int>();

            _mockChatRepository
                .Setup(x => x.GetIndividualChatByAccountIdsAsync(
                    It.Is<List<int>>(ids => ids.SequenceEqual(accountIdsAsInt)), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Chat)null);

            _mockChatRepository
                .Setup(x => x.CreateAsync(It.IsAny<Chat>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateIndividualChatAsync(accountIds);

            // Assert
            result.Should().Be("0"); // Default ID for new entity
            _mockChatRepository.Verify(x => x.GetIndividualChatByAccountIdsAsync(
                It.Is<List<int>>(ids => ids.SequenceEqual(accountIdsAsInt)), 
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    [TestFixture]
    public class IsAccountChatMemberAsyncTests : ChatEntityFrameworkServiceGeneratedTests
    {
        [Test]
        public async Task IsAccountChatMemberAsync_WhenAccountIsMember_ShouldReturnTrue()
        {
            // Arrange
            const string chatId = "123";
            const string accountId = "456";
            const int chatIdAsInt = 123;
            const int accountIdAsInt = 456;

            _mockChatMemberRepository
                .Setup(x => x.IsAccountChatMemberAsync(chatIdAsInt, accountIdAsInt, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.IsAccountChatMemberAsync(chatId, accountId);

            // Assert
            result.Should().BeTrue();
            _mockChatMemberRepository.Verify(x => x.IsAccountChatMemberAsync(chatIdAsInt, accountIdAsInt, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task IsAccountChatMemberAsync_WhenAccountIsNotMember_ShouldReturnFalse()
        {
            // Arrange
            const string chatId = "789";
            const string accountId = "101";
            const int chatIdAsInt = 789;
            const int accountIdAsInt = 101;

            _mockChatMemberRepository
                .Setup(x => x.IsAccountChatMemberAsync(chatIdAsInt, accountIdAsInt, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.IsAccountChatMemberAsync(chatId, accountId);

            // Assert
            result.Should().BeFalse();
            _mockChatMemberRepository.Verify(x => x.IsAccountChatMemberAsync(chatIdAsInt, accountIdAsInt, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task IsAccountChatMemberAsync_WithCancellationToken_ShouldPassTokenToRepository()
        {
            // Arrange
            const string chatId = "111";
            const string accountId = "222";
            const int chatIdAsInt = 111;
            const int accountIdAsInt = 222;
            var cancellationToken = new CancellationToken();

            _mockChatMemberRepository
                .Setup(x => x.IsAccountChatMemberAsync(chatIdAsInt, accountIdAsInt, cancellationToken))
                .ReturnsAsync(true);

            // Act
            await _service.IsAccountChatMemberAsync(chatId, accountId, cancellationToken);

            // Assert
            _mockChatMemberRepository.Verify(x => x.IsAccountChatMemberAsync(chatIdAsInt, accountIdAsInt, cancellationToken), Times.Once);
        }

        [Test]
        public void IsAccountChatMemberAsync_WithInvalidChatId_ShouldThrowFormatException()
        {
            // Arrange
            const string invalidChatId = "invalid";
            const string accountId = "123";

            // Act & Assert
            var act = async () => await _service.IsAccountChatMemberAsync(invalidChatId, accountId);
            act.Should().ThrowAsync<FormatException>();
        }

        [Test]
        public void IsAccountChatMemberAsync_WithInvalidAccountId_ShouldThrowFormatException()
        {
            // Arrange
            const string chatId = "123";
            const string invalidAccountId = "invalid";

            // Act & Assert
            var act = async () => await _service.IsAccountChatMemberAsync(chatId, invalidAccountId);
            act.Should().ThrowAsync<FormatException>();
        }

        [Test]
        public void IsAccountChatMemberAsync_WithBothInvalidIds_ShouldThrowFormatException()
        {
            // Arrange
            const string invalidChatId = "invalid-chat";
            const string invalidAccountId = "invalid-account";

            // Act & Assert
            var act = async () => await _service.IsAccountChatMemberAsync(invalidChatId, invalidAccountId);
            act.Should().ThrowAsync<FormatException>();
        }
    }
}
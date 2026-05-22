using AutoMapper;
using FluentAssertions;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.MongoDB.Models;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;
using Moq;

namespace LetsTalk.Server.Persistence.MongoDB.Services.Tests;

[TestFixture]
public class AccountMongoDBServiceTests
{
    private Mock<IAccountRepository> _mockAccountRepository;
    private Mock<IMapper> _mockMapper;
    private AccountMongoDBService _service;

    [SetUp]
    public void SetUp()
    {
        _mockAccountRepository = new Mock<IAccountRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new AccountMongoDBService(_mockAccountRepository.Object, _mockMapper.Object);
    }

    [TestFixture]
    public class GetOrCreateAsyncTests : AccountMongoDBServiceTests
    {
        [Test]
        public async Task GetOrCreateAsync_WhenAccountExists_ShouldReturnExistingAccountId()
        {
            // Arrange
            const AccountTypes accountType = AccountTypes.Email;
            const string email = "john.doe@example.com";
            var existingAccount = new Account
            {
                Id = "507f1f77bcf86cd799439011",
                Email = email,
                AccountTypeId = (int)accountType
            };

            _mockAccountRepository
                .Setup(x => x.GetByEmailAsync(email, accountType, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingAccount);

            // Act
            var result = await _service.GetOrCreateAsync(accountType, email);

            // Assert
            result.Should().Be(existingAccount.Id);
            _mockAccountRepository.Verify(x => x.GetByEmailAsync(email, accountType, It.IsAny<CancellationToken>()), Times.Once);
            _mockAccountRepository.Verify(x => x.CreateAccountAsync(It.IsAny<AccountTypes>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task GetOrCreateAsync_WhenAccountDoesNotExist_ShouldCreateNewAccountAndReturnId()
        {
            // Arrange
            const AccountTypes accountType = AccountTypes.Facebook;
            const string email = "new.user@example.com";
            var newAccount = new Account
            {
                Id = "507f1f77bcf86cd799439012",
                Email = email,
                AccountTypeId = (int)accountType
            };

            _mockAccountRepository
                .Setup(x => x.GetByEmailAsync(email, accountType, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account)null!);

            _mockAccountRepository
                .Setup(x => x.CreateAccountAsync(accountType, email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(newAccount);

            // Act
            var result = await _service.GetOrCreateAsync(accountType, email);

            // Assert
            result.Should().Be(newAccount.Id);
            _mockAccountRepository.Verify(x => x.GetByEmailAsync(email, accountType, It.IsAny<CancellationToken>()), Times.Once);
            _mockAccountRepository.Verify(x => x.CreateAccountAsync(accountType, email, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetOrCreateAsync_WhenCreateThrowsException_ShouldRetryGetByEmailAndReturnId()
        {
            // Arrange
            const AccountTypes accountType = AccountTypes.VK;
            const string email = "concurrent.user@example.com";
            var existingAccount = new Account
            {
                Id = "507f1f77bcf86cd799439013",
                Email = email,
                AccountTypeId = (int)accountType
            };

            _mockAccountRepository
                .SetupSequence(x => x.GetByEmailAsync(email, accountType, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account)null!)  // First call returns null
                .ReturnsAsync(existingAccount);  // Second call (after exception) returns existing account

            _mockAccountRepository
                .Setup(x => x.CreateAccountAsync(accountType, email, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Duplicate key"));

            // Act
            var result = await _service.GetOrCreateAsync(accountType, email);

            // Assert
            result.Should().Be(existingAccount.Id);
            _mockAccountRepository.Verify(x => x.GetByEmailAsync(email, accountType, It.IsAny<CancellationToken>()), Times.Exactly(2));
            _mockAccountRepository.Verify(x => x.CreateAccountAsync(accountType, email, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetOrCreateAsync_WithCancellationToken_ShouldPassTokenToAllMethods()
        {
            // Arrange
            const AccountTypes accountType = AccountTypes.Email;
            const string email = "test@example.com";
            var cancellationToken = new CancellationToken();
            var existingAccount = new Account { Id = "507f1f77bcf86cd799439011" };

            _mockAccountRepository
                .Setup(x => x.GetByEmailAsync(email, accountType, cancellationToken))
                .ReturnsAsync(existingAccount);

            // Act
            await _service.GetOrCreateAsync(accountType, email, cancellationToken);

            // Assert
            _mockAccountRepository.Verify(x => x.GetByEmailAsync(email, accountType, cancellationToken), Times.Once);
        }

        [Test]
        public async Task GetOrCreateAsync_WithDifferentAccountTypes_ShouldHandleAllTypes()
        {
            // Arrange
            const AccountTypes accountType = AccountTypes.VK;
            const string email = "vk.user@example.com";
            var newAccount = new Account
            {
                Id = "507f1f77bcf86cd799439014",
                Email = email,
                AccountTypeId = (int)accountType
            };

            _mockAccountRepository
                .Setup(x => x.GetByEmailAsync(email, accountType, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account)null!);

            _mockAccountRepository
                .Setup(x => x.CreateAccountAsync(accountType, email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(newAccount);

            // Act
            var result = await _service.GetOrCreateAsync(accountType, email);

            // Assert
            result.Should().Be(newAccount.Id);
            _mockAccountRepository.Verify(x => x.CreateAccountAsync(accountType, email, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetOrCreateAsync_WithNullEmail_ShouldPassNullToRepository()
        {
            // Arrange
            const AccountTypes accountType = AccountTypes.Email;
            string email = null!;
            var account = new Account { Id = "507f1f77bcf86cd799439015" };

            _mockAccountRepository
                .Setup(x => x.GetByEmailAsync(email, accountType, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            // Act
            var result = await _service.GetOrCreateAsync(accountType, email);

            // Assert
            result.Should().Be(account.Id);
            _mockAccountRepository.Verify(x => x.GetByEmailAsync(email, accountType, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetOrCreateAsync_WhenCreateFailsAndRetryReturnsNull_ShouldThrowNullReferenceException()
        {
            // Arrange
            const AccountTypes accountType = AccountTypes.Email;
            const string email = "problematic@example.com";

            _mockAccountRepository
                .Setup(x => x.GetByEmailAsync(email, accountType, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account)null!);

            _mockAccountRepository
                .Setup(x => x.CreateAccountAsync(accountType, email, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Creation failed"));

            // Act & Assert
            var act = async () => await _service.GetOrCreateAsync(accountType, email);
            await act.Should().ThrowAsync<NullReferenceException>();
            
            _mockAccountRepository.Verify(x => x.GetByEmailAsync(email, accountType, It.IsAny<CancellationToken>()), Times.Exactly(2));
            _mockAccountRepository.Verify(x => x.CreateAccountAsync(accountType, email, It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    [TestFixture]
    public class GetAccountsAsyncTests : AccountMongoDBServiceTests
    {
        [Test]
        public async Task GetAccountsAsync_WithAccounts_ShouldReturnMappedAccountList()
        {
            // Arrange
            var accounts = new List<Account>
            {
                new() {
                    Id = "507f1f77bcf86cd799439011",
                    FirstName = "John",
                    LastName = "Doe",
                    AccountTypeId = (int)AccountTypes.Email
                },
                new() {
                    Id = "507f1f77bcf86cd799439012",
                    FirstName = "Jane",
                    LastName = "Smith",
                    AccountTypeId = (int)AccountTypes.Facebook
                }
            };

            var expectedAccountModels = new List<AccountServiceModel>
            {
                new() {
                    Id = "507f1f77bcf86cd799439011",
                    FirstName = "John",
                    LastName = "Doe",
                    AccountTypeId = (int)AccountTypes.Email
                },
                new() {
                    Id = "507f1f77bcf86cd799439012",
                    FirstName = "Jane",
                    LastName = "Smith",
                    AccountTypeId = (int)AccountTypes.Facebook
                }
            };

            _mockAccountRepository
                .Setup(x => x.GetAccountsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(accounts);

            _mockMapper
                .Setup(x => x.Map<List<AccountServiceModel>>(accounts))
                .Returns(expectedAccountModels);

            // Act
            var result = await _service.GetAccountsAsync();

            // Assert
            result.Should().BeEquivalentTo(expectedAccountModels);
            _mockAccountRepository.Verify(x => x.GetAccountsAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(x => x.Map<List<AccountServiceModel>>(accounts), Times.Once);
        }

        [Test]
        public async Task GetAccountsAsync_WithEmptyList_ShouldReturnEmptyMappedList()
        {
            // Arrange
            var accounts = new List<Account>();
            var expectedAccountModels = new List<AccountServiceModel>();

            _mockAccountRepository
                .Setup(x => x.GetAccountsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(accounts);

            _mockMapper
                .Setup(x => x.Map<List<AccountServiceModel>>(accounts))
                .Returns(expectedAccountModels);

            // Act
            var result = await _service.GetAccountsAsync();

            // Assert
            result.Should().BeEmpty();
            _mockAccountRepository.Verify(x => x.GetAccountsAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(x => x.Map<List<AccountServiceModel>>(accounts), Times.Once);
        }

        [Test]
        public async Task GetAccountsAsync_WithCancellationToken_ShouldPassTokenToRepository()
        {
            // Arrange
            var cancellationToken = new CancellationToken();
            var accounts = new List<Account>();
            var expectedAccountModels = new List<AccountServiceModel>();

            _mockAccountRepository
                .Setup(x => x.GetAccountsAsync(cancellationToken))
                .ReturnsAsync(accounts);

            _mockMapper
                .Setup(x => x.Map<List<AccountServiceModel>>(accounts))
                .Returns(expectedAccountModels);

            // Act
            await _service.GetAccountsAsync(cancellationToken);

            // Assert
            _mockAccountRepository.Verify(x => x.GetAccountsAsync(cancellationToken), Times.Once);
        }

        [Test]
        public async Task GetAccountsAsync_WithAccountsWithImages_ShouldReturnMappedAccountsWithImages()
        {
            // Arrange
            var accounts = new List<Account>
            {
                new() {
                    Id = "507f1f77bcf86cd799439011",
                    FirstName = "John",
                    LastName = "Doe",
                    Image = new Image
                    {
                        Id = "image123",
                        FileStorageTypeId = (int)FileStorageTypes.AmazonS3
                    }
                }
            };

            var expectedAccountModels = new List<AccountServiceModel>
            {
                new() {
                    Id = "507f1f77bcf86cd799439011",
                    FirstName = "John",
                    LastName = "Doe",
                    Image = new ImageServiceModel
                    {
                        Id = "image123",
                        FileStorageTypeId = (int)FileStorageTypes.AmazonS3
                    }
                }
            };

            _mockAccountRepository
                .Setup(x => x.GetAccountsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(accounts);

            _mockMapper
                .Setup(x => x.Map<List<AccountServiceModel>>(accounts))
                .Returns(expectedAccountModels);

            // Act
            var result = await _service.GetAccountsAsync();

            // Assert
            result.Should().BeEquivalentTo(expectedAccountModels);
            result.First().Image.Should().NotBeNull();
            result.First().Image!.Id.Should().Be("image123");
        }

        [Test]
        public async Task GetAccountsAsync_WithNullAccountsList_ShouldReturnMappedResult()
        {
            // Arrange
            List<Account> accounts = null!;
            List<AccountServiceModel> expectedAccountModels = null!;

            _mockAccountRepository
                .Setup(x => x.GetAccountsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(accounts);

            _mockMapper
                .Setup(x => x.Map<List<AccountServiceModel>>(accounts))
                .Returns(expectedAccountModels);

            // Act
            var result = await _service.GetAccountsAsync();

            // Assert
            result.Should().BeNull();
            _mockAccountRepository.Verify(x => x.GetAccountsAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(x => x.Map<List<AccountServiceModel>>(accounts), Times.Once);
        }
    }

    [TestFixture]
    public class IsAccountIdValidAsyncTests : AccountMongoDBServiceTests
    {
        [Test]
        public async Task IsAccountIdValidAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            const string accountId = "507f1f77bcf86cd799439011";

            _mockAccountRepository
                .Setup(x => x.IsAccountIdValidAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.IsAccountIdValidAsync(accountId);

            // Assert
            result.Should().BeTrue();
            _mockAccountRepository.Verify(x => x.IsAccountIdValidAsync(accountId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task IsAccountIdValidAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            const string accountId = "invalid-id";

            _mockAccountRepository
                .Setup(x => x.IsAccountIdValidAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.IsAccountIdValidAsync(accountId);

            // Assert
            result.Should().BeFalse();
            _mockAccountRepository.Verify(x => x.IsAccountIdValidAsync(accountId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task IsAccountIdValidAsync_WithCancellationToken_ShouldPassTokenToRepository()
        {
            // Arrange
            const string accountId = "507f1f77bcf86cd799439011";
            var cancellationToken = new CancellationToken();

            _mockAccountRepository
                .Setup(x => x.IsAccountIdValidAsync(accountId, cancellationToken))
                .ReturnsAsync(true);

            // Act
            await _service.IsAccountIdValidAsync(accountId, cancellationToken);

            // Assert
            _mockAccountRepository.Verify(x => x.IsAccountIdValidAsync(accountId, cancellationToken), Times.Once);
        }

        [Test]
        public async Task IsAccountIdValidAsync_WithNullId_ShouldPassNullToRepository()
        {
            // Arrange
            string accountId = null!;

            _mockAccountRepository
                .Setup(x => x.IsAccountIdValidAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.IsAccountIdValidAsync(accountId);

            // Assert
            result.Should().BeFalse();
            _mockAccountRepository.Verify(x => x.IsAccountIdValidAsync(accountId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task IsAccountIdValidAsync_WithEmptyString_ShouldPassEmptyStringToRepository()
        {
            // Arrange
            const string accountId = "";

            _mockAccountRepository
                .Setup(x => x.IsAccountIdValidAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.IsAccountIdValidAsync(accountId);

            // Assert
            result.Should().BeFalse();
            _mockAccountRepository.Verify(x => x.IsAccountIdValidAsync(accountId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task IsAccountIdValidAsync_WithWhitespaceString_ShouldPassWhitespaceToRepository()
        {
            // Arrange
            const string accountId = "   ";

            _mockAccountRepository
                .Setup(x => x.IsAccountIdValidAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.IsAccountIdValidAsync(accountId);

            // Assert
            result.Should().BeFalse();
            _mockAccountRepository.Verify(x => x.IsAccountIdValidAsync(accountId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task IsAccountIdValidAsync_WithValidObjectIdFormat_ShouldReturnTrue()
        {
            // Arrange
            const string accountId = "507f1f77bcf86cd799439011";

            _mockAccountRepository
                .Setup(x => x.IsAccountIdValidAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.IsAccountIdValidAsync(accountId);

            // Assert
            result.Should().BeTrue();
            _mockAccountRepository.Verify(x => x.IsAccountIdValidAsync(accountId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task IsAccountIdValidAsync_WithInvalidObjectIdFormat_ShouldReturnFalse()
        {
            // Arrange
            const string accountId = "not-an-object-id";

            _mockAccountRepository
                .Setup(x => x.IsAccountIdValidAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.IsAccountIdValidAsync(accountId);

            // Assert
            result.Should().BeFalse();
            _mockAccountRepository.Verify(x => x.IsAccountIdValidAsync(accountId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
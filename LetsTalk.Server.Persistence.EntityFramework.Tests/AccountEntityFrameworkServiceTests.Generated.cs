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
public class AccountEntityFrameworkServiceGeneratedTests
{
    private Mock<IAccountRepository> _mockAccountRepository;
    private Mock<IImageRepository> _mockImageRepository;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IMapper> _mockMapper;
    private Mock<IEntityFactory> _mockEntityFactory;
    private AccountEntityFrameworkService _service;

    [SetUp]
    public void SetUp()
    {
        _mockAccountRepository = new Mock<IAccountRepository>();
        _mockImageRepository = new Mock<IImageRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockEntityFactory = new Mock<IEntityFactory>();

        _service = new AccountEntityFrameworkService(
            _mockAccountRepository.Object,
            _mockImageRepository.Object,
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockEntityFactory.Object);
    }

    [TestFixture]
    public class GetByIdAsyncTests : AccountEntityFrameworkServiceGeneratedTests
    {
        [Test]
        public async Task GetByIdAsync_WithValidId_ShouldReturnMappedProfile()
        {
            // Arrange
            const string accountId = "123";
            const int accountIdAsInt = 123;
            var account = CreateAccountWithId(accountIdAsInt);
            var expectedProfile = new ProfileServiceModel { Id = accountId, FirstName = "John", LastName = "Doe" };

            _mockAccountRepository
                .Setup(x => x.GetByIdAsync(accountIdAsInt, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            _mockMapper
                .Setup(x => x.Map<ProfileServiceModel>(account))
                .Returns(expectedProfile);

            // Act
            var result = await _service.GetByIdAsync(accountId);

            // Assert
            result.Should().BeEquivalentTo(expectedProfile);
            _mockAccountRepository.Verify(x => x.GetByIdAsync(accountIdAsInt, It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(x => x.Map<ProfileServiceModel>(account), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_WithCancellationToken_ShouldPassTokenToRepository()
        {
            // Arrange
            const string accountId = "456";
            const int accountIdAsInt = 456;
            var cancellationToken = new CancellationToken();
            var account = CreateAccountWithId(accountIdAsInt);
            var expectedProfile = new ProfileServiceModel();

            _mockAccountRepository
                .Setup(x => x.GetByIdAsync(accountIdAsInt, cancellationToken))
                .ReturnsAsync(account);

            _mockMapper
                .Setup(x => x.Map<ProfileServiceModel>(account))
                .Returns(expectedProfile);

            // Act
            await _service.GetByIdAsync(accountId, cancellationToken);

            // Assert
            _mockAccountRepository.Verify(x => x.GetByIdAsync(accountIdAsInt, cancellationToken), Times.Once);
        }

        [Test]
        public void GetByIdAsync_WithInvalidId_ShouldThrowFormatException()
        {
            // Arrange
            const string invalidAccountId = "invalid";

            // Act & Assert
            var act = async () => await _service.GetByIdAsync(invalidAccountId);
            act.Should().ThrowAsync<FormatException>();
        }
    }

    [TestFixture]
    public class UpdateProfileAsyncTests : AccountEntityFrameworkServiceGeneratedTests
    {
        [Test]
        public async Task UpdateProfileAsync_WithoutImage_ShouldUpdateProfileAndReturnMappedResult()
        {
            // Arrange
            const string accountId = "123";
            const int accountIdAsInt = 123;
            const string firstName = "John";
            const string lastName = "Doe";
            var account = CreateAccountWithId(accountIdAsInt);
            var expectedProfile = new ProfileServiceModel { Id = accountId, FirstName = firstName, LastName = lastName };

            _mockAccountRepository
                .Setup(x => x.GetByIdAsTrackingAsync(accountIdAsInt, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            _mockUnitOfWork
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockMapper
                .Setup(x => x.Map<ProfileServiceModel>(account))
                .Returns(expectedProfile);

            // Act
            var result = await _service.UpdateProfileAsync(accountId, firstName, lastName);

            // Assert
            result.Should().BeEquivalentTo(expectedProfile);
            _mockAccountRepository.Verify(x => x.GetByIdAsTrackingAsync(accountIdAsInt, It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(x => x.Map<ProfileServiceModel>(account), Times.Once);
        }

        [Test]
        public async Task UpdateProfileAsync_WithImage_ShouldUpdateProfileWithImageAndReturnMappedResult()
        {
            // Arrange
            const string accountId = "123";
            const int accountIdAsInt = 123;
            const string firstName = "John";
            const string lastName = "Doe";
            const string imageId = "image123";
            const int width = 100;
            const int height = 100;
            const ImageFormats imageFormat = ImageFormats.Jpeg;
            const FileStorageTypes fileStorageType = FileStorageTypes.Local;

            var account = CreateAccountWithId(accountIdAsInt);
            var existingImage = CreateImage("oldImage");
            account.GetType().GetProperty("Image")?.SetValue(account, existingImage);

            var newImage = CreateImage(imageId);
            var expectedProfile = new ProfileServiceModel { Id = accountId, FirstName = firstName, LastName = lastName };

            _mockAccountRepository
                .Setup(x => x.GetByIdAsTrackingAsync(accountIdAsInt, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            _mockEntityFactory
                .Setup(x => x.CreateImage(imageId, imageFormat, width, height, fileStorageType))
                .Returns(newImage);

            _mockUnitOfWork
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockMapper
                .Setup(x => x.Map<ProfileServiceModel>(account))
                .Returns(expectedProfile);

            // Act
            var result = await _service.UpdateProfileAsync(accountId, firstName, lastName, imageId, width, height, imageFormat, fileStorageType);

            // Assert
            result.Should().BeEquivalentTo(expectedProfile);
            _mockAccountRepository.Verify(x => x.GetByIdAsTrackingAsync(accountIdAsInt, It.IsAny<CancellationToken>()), Times.Once);
            _mockEntityFactory.Verify(x => x.CreateImage(imageId, imageFormat, width, height, fileStorageType), Times.Once);
            _mockImageRepository.Verify(x => x.Delete(existingImage), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(x => x.Map<ProfileServiceModel>(account), Times.Once);
        }

        [Test]
        public async Task UpdateProfileAsync_WithImageButNoExistingImage_ShouldNotDeleteImage()
        {
            // Arrange
            const string accountId = "123";
            const int accountIdAsInt = 123;
            const string firstName = "John";
            const string lastName = "Doe";
            const string imageId = "image123";
            const int width = 100;
            const int height = 100;
            const ImageFormats imageFormat = ImageFormats.Jpeg;
            const FileStorageTypes fileStorageType = FileStorageTypes.Local;

            var account = CreateAccountWithId(accountIdAsInt);
            var newImage = CreateImage(imageId);
            var expectedProfile = new ProfileServiceModel();

            _mockAccountRepository
                .Setup(x => x.GetByIdAsTrackingAsync(accountIdAsInt, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            _mockEntityFactory
                .Setup(x => x.CreateImage(imageId, imageFormat, width, height, fileStorageType))
                .Returns(newImage);

            _mockUnitOfWork
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockMapper
                .Setup(x => x.Map<ProfileServiceModel>(account))
                .Returns(expectedProfile);

            // Act
            await _service.UpdateProfileAsync(accountId, firstName, lastName, imageId, width, height, imageFormat, fileStorageType);

            // Assert
            _mockImageRepository.Verify(x => x.Delete(It.IsAny<Image>()), Times.Never);
        }

        [Test]
        public async Task UpdateProfileAsync_WithEmptyImageId_ShouldNotDeleteExistingImage()
        {
            // Arrange
            const string accountId = "123";
            const int accountIdAsInt = 123;
            const string firstName = "John";
            const string lastName = "Doe";
            const string imageId = "";
            const int width = 100;
            const int height = 100;
            const ImageFormats imageFormat = ImageFormats.Jpeg;
            const FileStorageTypes fileStorageType = FileStorageTypes.Local;

            var account = CreateAccountWithId(accountIdAsInt);
            var existingImage = CreateImage("oldImage");
            account.GetType().GetProperty("Image")?.SetValue(account, existingImage);

            var newImage = CreateImage(imageId);
            var expectedProfile = new ProfileServiceModel();

            _mockAccountRepository
                .Setup(x => x.GetByIdAsTrackingAsync(accountIdAsInt, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            _mockEntityFactory
                .Setup(x => x.CreateImage(imageId, imageFormat, width, height, fileStorageType))
                .Returns(newImage);

            _mockUnitOfWork
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockMapper
                .Setup(x => x.Map<ProfileServiceModel>(account))
                .Returns(expectedProfile);

            // Act
            await _service.UpdateProfileAsync(accountId, firstName, lastName, imageId, width, height, imageFormat, fileStorageType);

            // Assert
            _mockImageRepository.Verify(x => x.Delete(It.IsAny<Image>()), Times.Never);
        }

        [Test]
        public async Task UpdateProfileAsync_WithCancellationToken_ShouldPassTokenToAllMethods()
        {
            // Arrange
            const string accountId = "123";
            const int accountIdAsInt = 123;
            const string firstName = "John";
            const string lastName = "Doe";
            var cancellationToken = new CancellationToken();
            var account = CreateAccountWithId(accountIdAsInt);
            var expectedProfile = new ProfileServiceModel();

            _mockAccountRepository
                .Setup(x => x.GetByIdAsTrackingAsync(accountIdAsInt, cancellationToken))
                .ReturnsAsync(account);

            _mockUnitOfWork
                .Setup(x => x.SaveAsync(cancellationToken))
                .Returns(Task.CompletedTask);

            _mockMapper
                .Setup(x => x.Map<ProfileServiceModel>(account))
                .Returns(expectedProfile);

            // Act
            await _service.UpdateProfileAsync(accountId, firstName, lastName, cancellationToken);

            // Assert
            _mockAccountRepository.Verify(x => x.GetByIdAsTrackingAsync(accountIdAsInt, cancellationToken), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(cancellationToken), Times.Once);
        }

        [Test]
        public void UpdateProfileAsync_WithInvalidAccountId_ShouldThrowFormatException()
        {
            // Arrange
            const string invalidAccountId = "invalid";
            const string firstName = "John";
            const string lastName = "Doe";

            // Act & Assert
            var act = async () => await _service.UpdateProfileAsync(invalidAccountId, firstName, lastName);
            act.Should().ThrowAsync<FormatException>();
        }
    }

    [TestFixture]
    public class GetOrCreateAsyncTests : AccountEntityFrameworkServiceGeneratedTests
    {
        [Test]
        public async Task GetOrCreateAsync_WhenAccountExists_ShouldReturnExistingAccountId()
        {
            // Arrange
            const AccountTypes accountType = AccountTypes.Email;
            const string email = "test@example.com";
            var existingAccount = CreateAccountWithId(123);

            _mockAccountRepository
                .Setup(x => x.GetByEmailAsync(email, accountType, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingAccount);

            // Act
            var result = await _service.GetOrCreateAsync(accountType, email);

            // Assert
            result.Should().Be("123");
            _mockAccountRepository.Verify(x => x.GetByEmailAsync(email, accountType, It.IsAny<CancellationToken>()), Times.Once);
            _mockEntityFactory.Verify(x => x.CreateAccount(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            _mockAccountRepository.Verify(x => x.CreateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task GetOrCreateAsync_WhenAccountDoesNotExist_ShouldCreateNewAccountAndReturnId()
        {
            // Arrange
            const AccountTypes accountType = AccountTypes.Email;
            const string email = "test@example.com";
            var newAccount = CreateAccountWithId(456);

            _mockAccountRepository
                .Setup(x => x.GetByEmailAsync(email, accountType, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account)null);

            _mockEntityFactory
                .Setup(x => x.CreateAccount((int)accountType, email))
                .Returns(newAccount);

            _mockAccountRepository
                .Setup(x => x.CreateAsync(newAccount, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.GetOrCreateAsync(accountType, email);

            // Assert
            result.Should().Be("456");
            _mockAccountRepository.Verify(x => x.GetByEmailAsync(email, accountType, It.IsAny<CancellationToken>()), Times.Once);
            _mockEntityFactory.Verify(x => x.CreateAccount((int)accountType, email), Times.Once);
            _mockAccountRepository.Verify(x => x.CreateAsync(newAccount, It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetOrCreateAsync_WhenDbUpdateExceptionOccurs_ShouldRetryAndReturnExistingAccountId()
        {
            // Arrange
            const AccountTypes accountType = AccountTypes.Email;
            const string email = "test@example.com";
            var newAccount = CreateAccountWithId(456);
            var existingAccount = CreateAccountWithId(789);

            _mockAccountRepository
                .SetupSequence(x => x.GetByEmailAsync(email, accountType, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account)null)
                .ReturnsAsync(existingAccount);

            _mockEntityFactory
                .Setup(x => x.CreateAccount((int)accountType, email))
                .Returns(newAccount);

            _mockAccountRepository
                .Setup(x => x.CreateAsync(newAccount, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DbUpdateException("Duplicate key"));

            // Act
            var result = await _service.GetOrCreateAsync(accountType, email);

            // Assert
            result.Should().Be("789");
            _mockAccountRepository.Verify(x => x.GetByEmailAsync(email, accountType, It.IsAny<CancellationToken>()), Times.Exactly(2));
            _mockEntityFactory.Verify(x => x.CreateAccount((int)accountType, email), Times.Once);
            _mockAccountRepository.Verify(x => x.CreateAsync(newAccount, It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetOrCreateAsync_WithCancellationToken_ShouldPassTokenToAllMethods()
        {
            // Arrange
            const AccountTypes accountType = AccountTypes.Email;
            const string email = "test@example.com";
            var cancellationToken = new CancellationToken();
            var existingAccount = CreateAccountWithId(123);

            _mockAccountRepository
                .Setup(x => x.GetByEmailAsync(email, accountType, cancellationToken))
                .ReturnsAsync(existingAccount);

            // Act
            await _service.GetOrCreateAsync(accountType, email, cancellationToken);

            // Assert
            _mockAccountRepository.Verify(x => x.GetByEmailAsync(email, accountType, cancellationToken), Times.Once);
        }
    }

    [TestFixture]
    public class GetAccountsAsyncTests : AccountEntityFrameworkServiceGeneratedTests
    {
        [Test]
        public async Task GetAccountsAsync_ShouldReturnMappedAccounts()
        {
            // Arrange
            var accounts = new List<Account>
            {
                CreateAccountWithId(1),
                CreateAccountWithId(2),
                CreateAccountWithId(3)
            };

            var expectedAccountModels = new List<AccountServiceModel>
            {
                new() { Id = "1", FirstName = "John1" },
                new() { Id = "2", FirstName = "John2" },
                new() { Id = "3", FirstName = "John3" }
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
        public async Task GetAccountsAsync_WithEmptyResult_ShouldReturnEmptyList()
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
    }

    [TestFixture]
    public class IsAccountIdValidAsyncTests : AccountEntityFrameworkServiceGeneratedTests
    {
        [Test]
        public async Task IsAccountIdValidAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            const string accountId = "123";
            const int accountIdAsInt = 123;

            _mockAccountRepository
                .Setup(x => x.IsAccountIdValidAsync(accountIdAsInt, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.IsAccountIdValidAsync(accountId);

            // Assert
            result.Should().BeTrue();
            _mockAccountRepository.Verify(x => x.IsAccountIdValidAsync(accountIdAsInt, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task IsAccountIdValidAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            const string accountId = "999";
            const int accountIdAsInt = 999;

            _mockAccountRepository
                .Setup(x => x.IsAccountIdValidAsync(accountIdAsInt, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.IsAccountIdValidAsync(accountId);

            // Assert
            result.Should().BeFalse();
            _mockAccountRepository.Verify(x => x.IsAccountIdValidAsync(accountIdAsInt, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task IsAccountIdValidAsync_WithCancellationToken_ShouldPassTokenToRepository()
        {
            // Arrange
            const string accountId = "456";
            const int accountIdAsInt = 456;
            var cancellationToken = new CancellationToken();

            _mockAccountRepository
                .Setup(x => x.IsAccountIdValidAsync(accountIdAsInt, cancellationToken))
                .ReturnsAsync(true);

            // Act
            await _service.IsAccountIdValidAsync(accountId, cancellationToken);

            // Assert
            _mockAccountRepository.Verify(x => x.IsAccountIdValidAsync(accountIdAsInt, cancellationToken), Times.Once);
        }

        [Test]
        public void IsAccountIdValidAsync_WithInvalidIdFormat_ShouldThrowFormatException()
        {
            // Arrange
            const string invalidAccountId = "abc";

            // Act & Assert
            var act = async () => await _service.IsAccountIdValidAsync(invalidAccountId);
            act.Should().ThrowAsync<FormatException>();
        }
    }

    private static Account CreateAccountWithId(int id)
    {
        var account = new Account(1, "test@example.com");
        var idProperty = typeof(BaseEntity).GetProperty("Id");
        idProperty!.SetValue(account, id);
        return account;
    }

    private static Image CreateImage(string imageId)
    {
        var image = new Image(imageId, (int)ImageFormats.Jpeg, 100, 100, (int)FileStorageTypes.Local);
        return image;
    }
}
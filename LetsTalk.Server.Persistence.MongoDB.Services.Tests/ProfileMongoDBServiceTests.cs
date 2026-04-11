using AutoMapper;
using FluentAssertions;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.Persistence.MongoDB.Models;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;
using Moq;

namespace LetsTalk.Server.Persistence.MongoDB.Services.Tests;

[TestFixture]
public class ProfileMongoDBServiceTests
{
    private Mock<IAccountRepository> _mockAccountRepository;
    private Mock<IMapper> _mockMapper;
    private ProfileMongoDBService _service;

    [SetUp]
    public void SetUp()
    {
        _mockAccountRepository = new Mock<IAccountRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new ProfileMongoDBService(_mockAccountRepository.Object, _mockMapper.Object);
    }

    [TestFixture]
    public class GetByIdAsyncTests : ProfileMongoDBServiceTests
    {
        [Test]
        public async Task GetByIdAsync_WithValidId_ShouldReturnMappedProfile()
        {
            // Arrange
            const string accountId = "507f1f77bcf86cd799439011";
            var account = new Account
            {
                Id = accountId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                AccountTypeId = (int)AccountTypes.Email
            };

            var expectedProfile = new ProfileServiceModel
            {
                Id = accountId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                AccountTypeId = (int)AccountTypes.Email
            };

            _mockAccountRepository
                .Setup(x => x.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            _mockMapper
                .Setup(x => x.Map<ProfileServiceModel>(account))
                .Returns(expectedProfile);

            // Act
            var result = await _service.GetByIdAsync(accountId);

            // Assert
            result.Should().BeEquivalentTo(expectedProfile);
            _mockAccountRepository.Verify(x => x.GetByIdAsync(accountId, It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(x => x.Map<ProfileServiceModel>(account), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_WithNullAccount_ShouldReturnMappedResult()
        {
            // Arrange
            const string accountId = "507f1f77bcf86cd799439011";
            Account account = null!;
            ProfileServiceModel expectedProfile = null!;

            _mockAccountRepository
                .Setup(x => x.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            _mockMapper
                .Setup(x => x.Map<ProfileServiceModel>(account))
                .Returns(expectedProfile);

            // Act
            var result = await _service.GetByIdAsync(accountId);

            // Assert
            result.Should().BeNull();
            _mockAccountRepository.Verify(x => x.GetByIdAsync(accountId, It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(x => x.Map<ProfileServiceModel>(account), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_WithCancellationToken_ShouldPassTokenToRepository()
        {
            // Arrange
            const string accountId = "507f1f77bcf86cd799439011";
            var cancellationToken = new CancellationToken();
            var account = new Account { Id = accountId };
            var expectedProfile = new ProfileServiceModel { Id = accountId };

            _mockAccountRepository
                .Setup(x => x.GetByIdAsync(accountId, cancellationToken))
                .ReturnsAsync(account);

            _mockMapper
                .Setup(x => x.Map<ProfileServiceModel>(account))
                .Returns(expectedProfile);

            // Act
            await _service.GetByIdAsync(accountId, cancellationToken);

            // Assert
            _mockAccountRepository.Verify(x => x.GetByIdAsync(accountId, cancellationToken), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_WithNullId_ShouldPassNullToRepository()
        {
            // Arrange
            string accountId = null!;
            Account account = null!;
            ProfileServiceModel expectedProfile = null!;

            _mockAccountRepository
                .Setup(x => x.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            _mockMapper
                .Setup(x => x.Map<ProfileServiceModel>(account))
                .Returns(expectedProfile);

            // Act
            var result = await _service.GetByIdAsync(accountId);

            // Assert
            result.Should().BeNull();
            _mockAccountRepository.Verify(x => x.GetByIdAsync(accountId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_WithAccountWithImage_ShouldReturnMappedProfileWithImage()
        {
            // Arrange
            const string accountId = "507f1f77bcf86cd799439011";
            var account = new Account
            {
                Id = accountId,
                FirstName = "Jane",
                LastName = "Smith",
                Image = new Image
                {
                    Id = "image123",
                    FileStorageTypeId = (int)FileStorageTypes.AmazonS3,
                    Width = 200,
                    Height = 200
                }
            };

            var expectedProfile = new ProfileServiceModel
            {
                Id = accountId,
                FirstName = "Jane",
                LastName = "Smith",
                Image = new ImageServiceModel
                {
                    Id = "image123",
                    FileStorageTypeId = (int)FileStorageTypes.AmazonS3
                }
            };

            _mockAccountRepository
                .Setup(x => x.GetByIdAsync(accountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            _mockMapper
                .Setup(x => x.Map<ProfileServiceModel>(account))
                .Returns(expectedProfile);

            // Act
            var result = await _service.GetByIdAsync(accountId);

            // Assert
            result.Should().BeEquivalentTo(expectedProfile);
            result.Image.Should().NotBeNull();
            result.Image.Id.Should().Be("image123");
        }
    }

    [TestFixture]
    public class UpdateProfileAsyncWithoutImageTests : ProfileMongoDBServiceTests
    {
        [Test]
        public async Task UpdateProfileAsync_WithValidParameters_ShouldReturnMappedProfile()
        {
            // Arrange
            const string accountId = "507f1f77bcf86cd799439011";
            const string firstName = "John";
            const string lastName = "Doe";

            var updatedAccount = new Account
            {
                Id = accountId,
                FirstName = firstName,
                LastName = lastName,
                Email = "john.doe@example.com"
            };

            var expectedProfile = new ProfileServiceModel
            {
                Id = accountId,
                FirstName = firstName,
                LastName = lastName,
                Email = "john.doe@example.com"
            };

            _mockAccountRepository
                .Setup(x => x.UpdateProfileAsync(accountId, firstName, lastName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedAccount);

            _mockMapper
                .Setup(x => x.Map<ProfileServiceModel>(updatedAccount))
                .Returns(expectedProfile);

            // Act
            var result = await _service.UpdateProfileAsync(accountId, firstName, lastName);

            // Assert
            result.Should().BeEquivalentTo(expectedProfile);
            _mockAccountRepository.Verify(x => x.UpdateProfileAsync(accountId, firstName, lastName, It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(x => x.Map<ProfileServiceModel>(updatedAccount), Times.Once);
        }

        [Test]
        public async Task UpdateProfileAsync_WithCancellationToken_ShouldPassTokenToRepository()
        {
            // Arrange
            const string accountId = "507f1f77bcf86cd799439011";
            const string firstName = "John";
            const string lastName = "Doe";
            var cancellationToken = new CancellationToken();

            var updatedAccount = new Account { Id = accountId, FirstName = firstName, LastName = lastName };
            var expectedProfile = new ProfileServiceModel { Id = accountId, FirstName = firstName, LastName = lastName };

            _mockAccountRepository
                .Setup(x => x.UpdateProfileAsync(accountId, firstName, lastName, cancellationToken))
                .ReturnsAsync(updatedAccount);

            _mockMapper
                .Setup(x => x.Map<ProfileServiceModel>(updatedAccount))
                .Returns(expectedProfile);

            // Act
            await _service.UpdateProfileAsync(accountId, firstName, lastName, cancellationToken);

            // Assert
            _mockAccountRepository.Verify(x => x.UpdateProfileAsync(accountId, firstName, lastName, cancellationToken), Times.Once);
        }

        [Test]
        public async Task UpdateProfileAsync_WithNullParameters_ShouldPassNullsToRepository()
        {
            // Arrange
            string accountId = null!;
            string firstName = null!;
            string lastName = null!;

            var updatedAccount = new Account();
            var expectedProfile = new ProfileServiceModel();

            _mockAccountRepository
                .Setup(x => x.UpdateProfileAsync(accountId, firstName, lastName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedAccount);

            _mockMapper
                .Setup(x => x.Map<ProfileServiceModel>(updatedAccount))
                .Returns(expectedProfile);

            // Act
            var result = await _service.UpdateProfileAsync(accountId, firstName, lastName);

            // Assert
            result.Should().BeEquivalentTo(expectedProfile);
            _mockAccountRepository.Verify(x => x.UpdateProfileAsync(accountId, firstName, lastName, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task UpdateProfileAsync_WithEmptyStrings_ShouldPassEmptyStringsToRepository()
        {
            // Arrange
            const string accountId = "";
            const string firstName = "";
            const string lastName = "";

            var updatedAccount = new Account { Id = accountId, FirstName = firstName, LastName = lastName };
            var expectedProfile = new ProfileServiceModel { Id = accountId, FirstName = firstName, LastName = lastName };

            _mockAccountRepository
                .Setup(x => x.UpdateProfileAsync(accountId, firstName, lastName, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedAccount);

            _mockMapper
                .Setup(x => x.Map<ProfileServiceModel>(updatedAccount))
                .Returns(expectedProfile);

            // Act
            var result = await _service.UpdateProfileAsync(accountId, firstName, lastName);

            // Assert
            result.Should().BeEquivalentTo(expectedProfile);
            _mockAccountRepository.Verify(x => x.UpdateProfileAsync(accountId, firstName, lastName, It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    [TestFixture]
    public class UpdateProfileAsyncWithImageTests : ProfileMongoDBServiceTests
    {
        [Test]
        public async Task UpdateProfileAsync_WithImageParameters_ShouldReturnMappedProfile()
        {
            // Arrange
            const string accountId = "507f1f77bcf86cd799439011";
            const string firstName = "John";
            const string lastName = "Doe";
            const string imageId = "image123";
            const int width = 200;
            const int height = 200;
            const ImageFormats imageFormat = ImageFormats.Jpeg;
            const FileStorageTypes fileStorageType = FileStorageTypes.AmazonS3;

            var updatedAccount = new Account
            {
                Id = accountId,
                FirstName = firstName,
                LastName = lastName,
                Image = new Image
                {
                    Id = imageId,
                    Width = width,
                    Height = height,
                    ImageFormatId = (int)imageFormat,
                    FileStorageTypeId = (int)fileStorageType
                }
            };

            var expectedProfile = new ProfileServiceModel
            {
                Id = accountId,
                FirstName = firstName,
                LastName = lastName,
                Image = new ImageServiceModel
                {
                    Id = imageId,
                    FileStorageTypeId = (int)fileStorageType
                }
            };

            _mockAccountRepository
                .Setup(x => x.UpdateProfileAsync(accountId, firstName, lastName, imageId, width, height, imageFormat, fileStorageType, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedAccount);

            _mockMapper
                .Setup(x => x.Map<ProfileServiceModel>(updatedAccount))
                .Returns(expectedProfile);

            // Act
            var result = await _service.UpdateProfileAsync(accountId, firstName, lastName, imageId, width, height, imageFormat, fileStorageType);

            // Assert
            result.Should().BeEquivalentTo(expectedProfile);
            _mockAccountRepository.Verify(x => x.UpdateProfileAsync(accountId, firstName, lastName, imageId, width, height, imageFormat, fileStorageType, It.IsAny<CancellationToken>()), Times.Once);
            _mockMapper.Verify(x => x.Map<ProfileServiceModel>(updatedAccount), Times.Once);
        }

        [Test]
        public async Task UpdateProfileAsync_WithImageAndCancellationToken_ShouldPassTokenToRepository()
        {
            // Arrange
            const string accountId = "507f1f77bcf86cd799439011";
            const string firstName = "John";
            const string lastName = "Doe";
            const string imageId = "image123";
            const int width = 200;
            const int height = 200;
            const ImageFormats imageFormat = ImageFormats.Png;
            const FileStorageTypes fileStorageType = FileStorageTypes.Local;
            var cancellationToken = new CancellationToken();

            var updatedAccount = new Account { Id = accountId };
            var expectedProfile = new ProfileServiceModel { Id = accountId };

            _mockAccountRepository
                .Setup(x => x.UpdateProfileAsync(accountId, firstName, lastName, imageId, width, height, imageFormat, fileStorageType, cancellationToken))
                .ReturnsAsync(updatedAccount);

            _mockMapper
                .Setup(x => x.Map<ProfileServiceModel>(updatedAccount))
                .Returns(expectedProfile);

            // Act
            await _service.UpdateProfileAsync(accountId, firstName, lastName, imageId, width, height, imageFormat, fileStorageType, cancellationToken);

            // Assert
            _mockAccountRepository.Verify(x => x.UpdateProfileAsync(accountId, firstName, lastName, imageId, width, height, imageFormat, fileStorageType, cancellationToken), Times.Once);
        }

        [Test]
        public async Task UpdateProfileAsync_WithDifferentImageFormats_ShouldHandleAllFormats()
        {
            // Arrange
            const string accountId = "507f1f77bcf86cd799439011";
            const string firstName = "John";
            const string lastName = "Doe";
            const string imageId = "image123";
            const int width = 150;
            const int height = 150;
            const ImageFormats imageFormat = ImageFormats.Webp;
            const FileStorageTypes fileStorageType = FileStorageTypes.AzureBlobStorage;

            var updatedAccount = new Account { Id = accountId, FirstName = firstName, LastName = lastName };
            var expectedProfile = new ProfileServiceModel { Id = accountId, FirstName = firstName, LastName = lastName };

            _mockAccountRepository
                .Setup(x => x.UpdateProfileAsync(accountId, firstName, lastName, imageId, width, height, imageFormat, fileStorageType, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedAccount);

            _mockMapper
                .Setup(x => x.Map<ProfileServiceModel>(updatedAccount))
                .Returns(expectedProfile);

            // Act
            var result = await _service.UpdateProfileAsync(accountId, firstName, lastName, imageId, width, height, imageFormat, fileStorageType);

            // Assert
            result.Should().BeEquivalentTo(expectedProfile);
            _mockAccountRepository.Verify(x => x.UpdateProfileAsync(accountId, firstName, lastName, imageId, width, height, imageFormat, fileStorageType, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task UpdateProfileAsync_WithZeroDimensions_ShouldPassZeroValuesToRepository()
        {
            // Arrange
            const string accountId = "507f1f77bcf86cd799439011";
            const string firstName = "John";
            const string lastName = "Doe";
            const string imageId = "image123";
            const int width = 0;
            const int height = 0;
            const ImageFormats imageFormat = ImageFormats.Gif;
            const FileStorageTypes fileStorageType = FileStorageTypes.Local;

            var updatedAccount = new Account { Id = accountId };
            var expectedProfile = new ProfileServiceModel { Id = accountId };

            _mockAccountRepository
                .Setup(x => x.UpdateProfileAsync(accountId, firstName, lastName, imageId, width, height, imageFormat, fileStorageType, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedAccount);

            _mockMapper
                .Setup(x => x.Map<ProfileServiceModel>(updatedAccount))
                .Returns(expectedProfile);

            // Act
            var result = await _service.UpdateProfileAsync(accountId, firstName, lastName, imageId, width, height, imageFormat, fileStorageType);

            // Assert
            result.Should().BeEquivalentTo(expectedProfile);
            _mockAccountRepository.Verify(x => x.UpdateProfileAsync(accountId, firstName, lastName, imageId, width, height, imageFormat, fileStorageType, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task UpdateProfileAsync_WithNullImageId_ShouldPassNullToRepository()
        {
            // Arrange
            const string accountId = "507f1f77bcf86cd799439011";
            const string firstName = "John";
            const string lastName = "Doe";
            string imageId = null!;
            const int width = 100;
            const int height = 100;
            const ImageFormats imageFormat = ImageFormats.Unknown;
            const FileStorageTypes fileStorageType = FileStorageTypes.Local;

            var updatedAccount = new Account { Id = accountId };
            var expectedProfile = new ProfileServiceModel { Id = accountId };

            _mockAccountRepository
                .Setup(x => x.UpdateProfileAsync(accountId, firstName, lastName, imageId, width, height, imageFormat, fileStorageType, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedAccount);

            _mockMapper
                .Setup(x => x.Map<ProfileServiceModel>(updatedAccount))
                .Returns(expectedProfile);

            // Act
            var result = await _service.UpdateProfileAsync(accountId, firstName, lastName, imageId, width, height, imageFormat, fileStorageType);

            // Assert
            result.Should().BeEquivalentTo(expectedProfile);
            _mockAccountRepository.Verify(x => x.UpdateProfileAsync(accountId, firstName, lastName, imageId, width, height, imageFormat, fileStorageType, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
using AutoMapper;
using FluentAssertions;
using LetsTalk.Server.Domain;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using Moq;

namespace LetsTalk.Server.Persistence.EntityFramework.Services.Tests;

[TestFixture]
public class ProfileEntityFrameworkServiceTests
{
    private Mock<IAccountRepository> _mockAccountRepository;
    private Mock<IImageRepository> _mockImageRepository;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IMapper> _mockMapper;
    private Mock<IEntityFactory> _mockEntityFactory;
    private ProfileEntityFrameworkService _service;

    [SetUp]
    public void SetUp()
    {
        _mockAccountRepository = new Mock<IAccountRepository>();
        _mockImageRepository = new Mock<IImageRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockEntityFactory = new Mock<IEntityFactory>();

        _service = new ProfileEntityFrameworkService(
            _mockAccountRepository.Object,
            _mockImageRepository.Object,
            _mockUnitOfWork.Object,
            _mockMapper.Object,
            _mockEntityFactory.Object);
    }

    [TestFixture]
    public class GetByIdAsyncTests : ProfileEntityFrameworkServiceTests
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
    public class UpdateProfileAsyncTests : ProfileEntityFrameworkServiceTests
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
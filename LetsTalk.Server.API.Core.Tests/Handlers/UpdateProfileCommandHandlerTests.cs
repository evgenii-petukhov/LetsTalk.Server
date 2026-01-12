using AutoMapper;
using FluentAssertions;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.API.Core.Features.Profile.Commands.UpdateProfile;
using LetsTalk.Server.API.Models.Message;
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
public class UpdateProfileCommandHandlerTests
{
    private Mock<IAccountAgnosticService> _accountAgnosticServiceMock;
    private Mock<IChatAgnosticService> _chatAgnosticServiceMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IProfileCacheManager> _profileCacheManagerMock;
    private Mock<IAccountCacheManager> _accountCacheManagerMock;
    private Mock<IChatCacheManager> _chatCacheManagerMock;
    private Mock<IProducer<RemoveImageRequest>> _producerMock;
    private UpdateProfileCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _accountAgnosticServiceMock = new Mock<IAccountAgnosticService>();
        _chatAgnosticServiceMock = new Mock<IChatAgnosticService>();
        _mapperMock = new Mock<IMapper>();
        _profileCacheManagerMock = new Mock<IProfileCacheManager>();
        _accountCacheManagerMock = new Mock<IAccountCacheManager>();
        _chatCacheManagerMock = new Mock<IChatCacheManager>();
        _producerMock = new Mock<IProducer<RemoveImageRequest>>();

        _handler = new UpdateProfileCommandHandler(
            _accountAgnosticServiceMock.Object,
            _chatAgnosticServiceMock.Object,
            _mapperMock.Object,
            _profileCacheManagerMock.Object,
            _accountCacheManagerMock.Object,
            _chatCacheManagerMock.Object,
            _producerMock.Object);
    }

    [Test]
    public async Task Handle_WhenUpdatingProfileWithoutImage_ShouldUpdateProfileAndClearCaches()
    {
        // Arrange
        var command = new UpdateProfileCommand
        {
            AccountId = "account-123",
            FirstName = "John",
            LastName = "Doe",
            Image = null
        };
        var cancellationToken = CancellationToken.None;

        var existingAccount = new ProfileServiceModel
        {
            Id = "account-123",
            FirstName = "OldFirst",
            LastName = "OldLast",
            Image = null
        };

        var updatedAccount = new ProfileServiceModel
        {
            Id = "account-123",
            FirstName = "John",
            LastName = "Doe",
            Image = null
        };

        var profileDto = new ProfileDto
        {
            Id = "account-123",
            FirstName = "John",
            LastName = "Doe"
        };

        SetupGetByIdAsync(command.AccountId!, existingAccount, cancellationToken);
        SetupUpdateProfileWithoutImage(command, updatedAccount, cancellationToken);
        SetupCacheOperations(command.AccountId!, cancellationToken);
        SetupMapper(updatedAccount, profileDto);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(profileDto);

        VerifyUpdateProfileWithoutImage(command, cancellationToken);
        VerifyBasicCacheOperations(command.AccountId!, cancellationToken);
        VerifyNoImageRemovalOperations(cancellationToken);
        VerifyMapper(updatedAccount);
    }

    [Test]
    public async Task Handle_WhenUpdatingProfileWithNewImage_ShouldUpdateProfileAndClearCaches()
    {
        // Arrange
        var command = new UpdateProfileCommand
        {
            AccountId = "account-123",
            FirstName = "John",
            LastName = "Doe",
            Image = new ImageRequestModel
            {
                Id = "image-456",
                Width = 200,
                Height = 200,
                ImageFormat = (int)ImageFormats.Jpeg,
                FileStorageTypeId = (int)FileStorageTypes.Local
            }
        };
        var cancellationToken = CancellationToken.None;

        var existingAccount = new ProfileServiceModel
        {
            Id = "account-123",
            FirstName = "OldFirst",
            LastName = "OldLast",
            Image = null
        };

        var updatedAccount = new ProfileServiceModel
        {
            Id = "account-123",
            FirstName = "John",
            LastName = "Doe",
            Image = new ImageServiceModel { Id = "image-456" }
        };

        var profileDto = new ProfileDto
        {
            Id = "account-123",
            FirstName = "John",
            LastName = "Doe"
        };

        SetupGetByIdAsync(command.AccountId!, existingAccount, cancellationToken);
        SetupUpdateProfileWithImage(command, updatedAccount, cancellationToken);
        SetupCacheOperations(command.AccountId!, cancellationToken);
        SetupMapper(updatedAccount, profileDto);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(profileDto);

        VerifyUpdateProfileWithImage(command, cancellationToken);
        VerifyBasicCacheOperations(command.AccountId!, cancellationToken);
        VerifyNoImageRemovalOperations(cancellationToken);
        VerifyMapper(updatedAccount);
    }

    [Test]
    public async Task Handle_WhenReplacingExistingImage_ShouldRemovePreviousImageAndUpdateProfile()
    {
        // Arrange
        var command = new UpdateProfileCommand
        {
            AccountId = "account-123",
            FirstName = "John",
            LastName = "Doe",
            Image = new ImageRequestModel
            {
                Id = "new-image-456",
                Width = 300,
                Height = 300,
                ImageFormat = (int)ImageFormats.Png,
                FileStorageTypeId = (int)FileStorageTypes.AmazonS3
            }
        };
        var cancellationToken = CancellationToken.None;

        var previousImage = new ImageServiceModel
        {
            Id = "old-image-123",
            FileStorageTypeId = (int)FileStorageTypes.Local
        };

        var existingAccount = new ProfileServiceModel
        {
            Id = "account-123",
            FirstName = "OldFirst",
            LastName = "OldLast",
            Image = previousImage
        };

        var updatedAccount = new ProfileServiceModel
        {
            Id = "account-123",
            FirstName = "John",
            LastName = "Doe",
            Image = new ImageServiceModel { Id = "new-image-456" }
        };

        var profileDto = new ProfileDto
        {
            Id = "account-123",
            FirstName = "John",
            LastName = "Doe"
        };

        var accountIds = new List<string> { "account-456", "account-789" };
        var removeImageRequest = new RemoveImageRequest
        {
            Id = "old-image-123",
            FileStorageTypeId = (int)FileStorageTypes.Local
        };

        SetupGetByIdAsync(command.AccountId!, existingAccount, cancellationToken);
        SetupUpdateProfileWithImage(command, updatedAccount, cancellationToken);
        SetupCacheOperations(command.AccountId!, cancellationToken);
        SetupImageRemovalOperations(command.AccountId!, accountIds, previousImage, removeImageRequest, cancellationToken);
        SetupMapper(updatedAccount, profileDto);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(profileDto);

        VerifyUpdateProfileWithImage(command, cancellationToken);
        VerifyBasicCacheOperations(command.AccountId!, cancellationToken);
        VerifyImageRemovalOperations(command.AccountId!, accountIds, previousImage, removeImageRequest, cancellationToken);
        VerifyMapper(updatedAccount);
    }

    [Test]
    public async Task Handle_WhenRemovingExistingImage_ShouldNotTriggerImageRemoval()
    {
        // Arrange
        var command = new UpdateProfileCommand
        {
            AccountId = "account-123",
            FirstName = "John",
            LastName = "Doe",
            Image = null
        };
        var cancellationToken = CancellationToken.None;

        var previousImage = new ImageServiceModel
        {
            Id = "old-image-123",
            FileStorageTypeId = (int)FileStorageTypes.Local
        };

        var existingAccount = new ProfileServiceModel
        {
            Id = "account-123",
            FirstName = "OldFirst",
            LastName = "OldLast",
            Image = previousImage
        };

        var updatedAccount = new ProfileServiceModel
        {
            Id = "account-123",
            FirstName = "John",
            LastName = "Doe",
            Image = null
        };

        var profileDto = new ProfileDto
        {
            Id = "account-123",
            FirstName = "John",
            LastName = "Doe"
        };

        SetupGetByIdAsync(command.AccountId!, existingAccount, cancellationToken);
        SetupUpdateProfileWithoutImage(command, updatedAccount, cancellationToken);
        SetupCacheOperations(command.AccountId!, cancellationToken);
        SetupMapper(updatedAccount, profileDto);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(profileDto);

        VerifyUpdateProfileWithoutImage(command, cancellationToken);
        VerifyBasicCacheOperations(command.AccountId!, cancellationToken);
        VerifyNoImageRemovalOperations(cancellationToken);
        VerifyMapper(updatedAccount);
    }

    [Test]
    public async Task Handle_WhenMultipleAccountsInChats_ShouldClearAllChatCaches()
    {
        // Arrange
        var command = new UpdateProfileCommand
        {
            AccountId = "account-123",
            FirstName = "John",
            LastName = "Doe",
            Image = new ImageRequestModel
            {
                Id = "new-image-456",
                Width = 100,
                Height = 100,
                ImageFormat = (int)ImageFormats.Webp,
                FileStorageTypeId = (int)FileStorageTypes.AmazonS3
            }
        };
        var cancellationToken = CancellationToken.None;

        var previousImage = new ImageServiceModel { Id = "old-image-123" };
        var existingAccount = new ProfileServiceModel { Id = "account-123", Image = previousImage };
        var updatedAccount = new ProfileServiceModel { Id = "account-123" };
        var profileDto = new ProfileDto { Id = "account-123" };

        var accountIds = new List<string> { "account-1", "account-2", "account-3", "account-4", "account-5" };
        var removeImageRequest = new RemoveImageRequest { Id = "old-image-123" };

        SetupGetByIdAsync(command.AccountId!, existingAccount, cancellationToken);
        SetupUpdateProfileWithImage(command, updatedAccount, cancellationToken);
        SetupCacheOperations(command.AccountId!, cancellationToken);
        SetupImageRemovalOperations(command.AccountId!, accountIds, previousImage, removeImageRequest, cancellationToken);
        SetupMapper(updatedAccount, profileDto);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(profileDto);

        foreach (var accountId in accountIds)
        {
            _chatCacheManagerMock.Verify(
                x => x.ClearAsync(accountId),
                Times.Once);
        }

        _chatCacheManagerMock.Verify(
            x => x.ClearAsync(It.IsAny<string>()),
            Times.Exactly(5));
    }

    [Test]
    public void Handle_WhenGetByIdAsyncThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new UpdateProfileCommand
        {
            AccountId = "account-123",
            FirstName = "John",
            LastName = "Doe"
        };
        var cancellationToken = CancellationToken.None;

        _accountAgnosticServiceMock
            .Setup(x => x.GetByIdAsync("account-123", cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Account not found"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));

        _accountAgnosticServiceMock.Verify(
            x => x.UpdateProfileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _profileCacheManagerMock.Verify(
            x => x.ClearAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Test]
    public void Handle_WhenUpdateProfileAsyncThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new UpdateProfileCommand
        {
            AccountId = "account-123",
            FirstName = "John",
            LastName = "Doe"
        };
        var cancellationToken = CancellationToken.None;

        var existingAccount = new ProfileServiceModel { Id = "account-123" };

        SetupGetByIdAsync(command.AccountId!, existingAccount, cancellationToken);

        _accountAgnosticServiceMock
            .Setup(x => x.UpdateProfileAsync("account-123", "John", "Doe", cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Update failed"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));

        _profileCacheManagerMock.Verify(
            x => x.ClearAsync(It.IsAny<string>()),
            Times.Never);

        _mapperMock.Verify(
            x => x.Map<ProfileDto>(It.IsAny<ProfileServiceModel>()),
            Times.Never);
    }

    [Test]
    public void Handle_WhenCacheClearingFails_ShouldPropagateException()
    {
        // Arrange
        var command = new UpdateProfileCommand
        {
            AccountId = "account-123",
            FirstName = "John",
            LastName = "Doe"
        };
        var cancellationToken = CancellationToken.None;

        var existingAccount = new ProfileServiceModel { Id = "account-123" };
        var updatedAccount = new ProfileServiceModel { Id = "account-123" };

        SetupGetByIdAsync(command.AccountId!, existingAccount, cancellationToken);
        SetupUpdateProfileWithoutImage(command, updatedAccount, cancellationToken);

        _profileCacheManagerMock
            .Setup(x => x.ClearAsync("account-123"))
            .ThrowsAsync(new InvalidOperationException("Cache clearing failed"));

        _accountCacheManagerMock
            .Setup(x => x.ClearAsync())
            .Returns(Task.CompletedTask);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));

        _mapperMock.Verify(
            x => x.Map<ProfileDto>(It.IsAny<ProfileServiceModel>()),
            Times.Never);
    }

    [Test]
    public void Handle_WhenImageRemovalPublishingFails_ShouldPropagateException()
    {
        // Arrange
        var command = new UpdateProfileCommand
        {
            AccountId = "account-123",
            FirstName = "John",
            LastName = "Doe",
            Image = new ImageRequestModel { Id = "new-image" }
        };
        var cancellationToken = CancellationToken.None;

        var previousImage = new ImageServiceModel { Id = "old-image" };
        var existingAccount = new ProfileServiceModel { Id = "account-123", Image = previousImage };
        var updatedAccount = new ProfileServiceModel { Id = "account-123" };

        var accountIds = new List<string> { "account-1" };
        var removeImageRequest = new RemoveImageRequest { Id = "old-image" };

        SetupGetByIdAsync(command.AccountId!, existingAccount, cancellationToken);
        SetupUpdateProfileWithImage(command, updatedAccount, cancellationToken);
        SetupCacheOperations(command.AccountId!, cancellationToken);

        _chatAgnosticServiceMock
            .Setup(x => x.GetAccountIdsInIndividualChatsAsync("account-123", cancellationToken))
            .ReturnsAsync(accountIds);

        _mapperMock
            .Setup(x => x.Map<RemoveImageRequest>(previousImage))
            .Returns(removeImageRequest);

        _producerMock
            .Setup(x => x.PublishAsync(removeImageRequest, cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Publishing failed"));

        _chatCacheManagerMock
            .Setup(x => x.ClearAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public void Handle_WhenMapperThrowsException_ShouldPropagateException()
    {
        // Arrange
        var command = new UpdateProfileCommand
        {
            AccountId = "account-123",
            FirstName = "John",
            LastName = "Doe"
        };
        var cancellationToken = CancellationToken.None;

        var existingAccount = new ProfileServiceModel { Id = "account-123" };
        var updatedAccount = new ProfileServiceModel { Id = "account-123" };

        SetupGetByIdAsync(command.AccountId!, existingAccount, cancellationToken);
        SetupUpdateProfileWithoutImage(command, updatedAccount, cancellationToken);
        SetupCacheOperations(command.AccountId!, cancellationToken);

        _mapperMock
            .Setup(x => x.Map<ProfileDto>(updatedAccount))
            .Throws(new InvalidOperationException("Mapping failed"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public async Task Handle_WhenCancellationTokenProvided_ShouldPassItToAllServices()
    {
        // Arrange
        var command = new UpdateProfileCommand
        {
            AccountId = "account-123",
            FirstName = "John",
            LastName = "Doe"
        };
        var cancellationToken = new CancellationToken(false);

        var existingAccount = new ProfileServiceModel { Id = "account-123" };
        var updatedAccount = new ProfileServiceModel { Id = "account-123" };
        var profileDto = new ProfileDto { Id = "account-123" };

        SetupGetByIdAsync(command.AccountId!, existingAccount, cancellationToken);
        SetupUpdateProfileWithoutImage(command, updatedAccount, cancellationToken);
        SetupCacheOperations(command.AccountId!, cancellationToken);
        SetupMapper(updatedAccount, profileDto);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert - Verification is done in the setup methods that check cancellationToken
        _accountAgnosticServiceMock.Verify(
            x => x.GetByIdAsync("account-123", cancellationToken),
            Times.Once);

        _accountAgnosticServiceMock.Verify(
            x => x.UpdateProfileAsync("account-123", "John", "Doe", cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenNullProperties_ShouldPassNullValues()
    {
        // Arrange
        var command = new UpdateProfileCommand
        {
            AccountId = null,
            FirstName = null,
            LastName = null,
            Image = null
        };
        var cancellationToken = CancellationToken.None;

        var existingAccount = new ProfileServiceModel { Id = null };
        var updatedAccount = new ProfileServiceModel { Id = null };
        var profileDto = new ProfileDto { Id = null };

        _accountAgnosticServiceMock
            .Setup(x => x.GetByIdAsync(null!, cancellationToken))
            .ReturnsAsync(existingAccount);

        _accountAgnosticServiceMock
            .Setup(x => x.UpdateProfileAsync(null!, null!, null!, cancellationToken))
            .ReturnsAsync(updatedAccount);

        SetupCacheOperations(null!, cancellationToken);
        SetupMapper(updatedAccount, profileDto);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(profileDto);

        _accountAgnosticServiceMock.Verify(
            x => x.UpdateProfileAsync(null!, null!, null!, cancellationToken),
            Times.Once);

        _profileCacheManagerMock.Verify(
            x => x.ClearAsync(null!),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenSpecialCharactersInProperties_ShouldHandleCorrectly()
    {
        // Arrange
        var command = new UpdateProfileCommand
        {
            AccountId = "account-!@#$%",
            FirstName = "John-O'Connor",
            LastName = "Müller & Sons",
            Image = null
        };
        var cancellationToken = CancellationToken.None;

        var existingAccount = new ProfileServiceModel { Id = "account-!@#$%" };
        var updatedAccount = new ProfileServiceModel 
        { 
            Id = "account-!@#$%",
            FirstName = "John-O'Connor",
            LastName = "Müller & Sons"
        };
        var profileDto = new ProfileDto 
        { 
            Id = "account-!@#$%",
            FirstName = "John-O'Connor",
            LastName = "Müller & Sons"
        };

        SetupGetByIdAsync(command.AccountId!, existingAccount, cancellationToken);
        
        _accountAgnosticServiceMock
            .Setup(x => x.UpdateProfileAsync("account-!@#$%", "John-O'Connor", "Müller & Sons", cancellationToken))
            .ReturnsAsync(updatedAccount);

        SetupCacheOperations(command.AccountId!, cancellationToken);
        SetupMapper(updatedAccount, profileDto);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(profileDto);

        _accountAgnosticServiceMock.Verify(
            x => x.UpdateProfileAsync("account-!@#$%", "John-O'Connor", "Müller & Sons", cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenUnicodeCharactersInProperties_ShouldHandleCorrectly()
    {
        // Arrange
        var command = new UpdateProfileCommand
        {
            AccountId = "账户-123",
            FirstName = "张三",
            LastName = "李四 🌟",
            Image = null
        };
        var cancellationToken = CancellationToken.None;

        var existingAccount = new ProfileServiceModel { Id = "账户-123" };
        var updatedAccount = new ProfileServiceModel 
        { 
            Id = "账户-123",
            FirstName = "张三",
            LastName = "李四 🌟"
        };
        var profileDto = new ProfileDto 
        { 
            Id = "账户-123",
            FirstName = "张三",
            LastName = "李四 🌟"
        };

        SetupGetByIdAsync(command.AccountId!, existingAccount, cancellationToken);
        
        _accountAgnosticServiceMock
            .Setup(x => x.UpdateProfileAsync("账户-123", "张三", "李四 🌟", cancellationToken))
            .ReturnsAsync(updatedAccount);

        SetupCacheOperations(command.AccountId!, cancellationToken);
        SetupMapper(updatedAccount, profileDto);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(profileDto);

        _accountAgnosticServiceMock.Verify(
            x => x.UpdateProfileAsync("账户-123", "张三", "李四 🌟", cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenEmptyAccountIdsList_ShouldNotClearChatCaches()
    {
        // Arrange
        var command = new UpdateProfileCommand
        {
            AccountId = "account-123",
            FirstName = "John",
            LastName = "Doe",
            Image = new ImageRequestModel { Id = "new-image" }
        };
        var cancellationToken = CancellationToken.None;

        var previousImage = new ImageServiceModel { Id = "old-image" };
        var existingAccount = new ProfileServiceModel { Id = "account-123", Image = previousImage };
        var updatedAccount = new ProfileServiceModel { Id = "account-123" };
        var profileDto = new ProfileDto { Id = "account-123" };

        var emptyAccountIds = new List<string>();
        var removeImageRequest = new RemoveImageRequest { Id = "old-image" };

        SetupGetByIdAsync(command.AccountId!, existingAccount, cancellationToken);
        SetupUpdateProfileWithImage(command, updatedAccount, cancellationToken);
        SetupCacheOperations(command.AccountId!, cancellationToken);
        SetupImageRemovalOperations(command.AccountId!, emptyAccountIds, previousImage, removeImageRequest, cancellationToken);
        SetupMapper(updatedAccount, profileDto);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(profileDto);

        _chatCacheManagerMock.Verify(
            x => x.ClearAsync(It.IsAny<string>()),
            Times.Never);

        _producerMock.Verify(
            x => x.PublishAsync(removeImageRequest, cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenDifferentImageFormats_ShouldHandleCorrectly()
    {
        // Arrange
        var testCases = new[]
        {
            new { Format = ImageFormats.Jpeg, StorageType = FileStorageTypes.Local },
            new { Format = ImageFormats.Png, StorageType = FileStorageTypes.AmazonS3 },
            new { Format = ImageFormats.Webp, StorageType = FileStorageTypes.Local },
            new { Format = ImageFormats.Gif, StorageType = FileStorageTypes.AmazonS3 }
        };

        foreach (var testCase in testCases)
        {
            var command = new UpdateProfileCommand
            {
                AccountId = "account-123",
                FirstName = "John",
                LastName = "Doe",
                Image = new ImageRequestModel
                {
                    Id = "image-456",
                    Width = 150,
                    Height = 150,
                    ImageFormat = (int)testCase.Format,
                    FileStorageTypeId = (int)testCase.StorageType
                }
            };
            var cancellationToken = CancellationToken.None;

            var existingAccount = new ProfileServiceModel { Id = "account-123" };
            var updatedAccount = new ProfileServiceModel { Id = "account-123" };
            var profileDto = new ProfileDto { Id = "account-123" };

            SetupGetByIdAsync(command.AccountId!, existingAccount, cancellationToken);
            SetupUpdateProfileWithImage(command, updatedAccount, cancellationToken);
            SetupCacheOperations(command.AccountId!, cancellationToken);
            SetupMapper(updatedAccount, profileDto);

            // Act
            var result = await _handler.Handle(command, cancellationToken);

            // Assert
            result.Should().BeEquivalentTo(profileDto);

            _accountAgnosticServiceMock.Verify(
                x => x.UpdateProfileAsync(
                    "account-123",
                    "John",
                    "Doe",
                    "image-456",
                    150,
                    150,
                    testCase.Format,
                    testCase.StorageType,
                    cancellationToken),
                Times.Once);

            // Reset mocks for next iteration
            _accountAgnosticServiceMock.Reset();
            _mapperMock.Reset();
            _profileCacheManagerMock.Reset();
            _accountCacheManagerMock.Reset();
        }
    }

    private void SetupGetByIdAsync(string accountId, ProfileServiceModel account, CancellationToken cancellationToken)
    {
        _accountAgnosticServiceMock
            .Setup(x => x.GetByIdAsync(accountId, cancellationToken))
            .ReturnsAsync(account);
    }

    private void SetupUpdateProfileWithoutImage(UpdateProfileCommand command, ProfileServiceModel updatedAccount, CancellationToken cancellationToken)
    {
        _accountAgnosticServiceMock
            .Setup(x => x.UpdateProfileAsync(command.AccountId!, command.FirstName!, command.LastName!, cancellationToken))
            .ReturnsAsync(updatedAccount);
    }

    private void SetupUpdateProfileWithImage(UpdateProfileCommand command, ProfileServiceModel updatedAccount, CancellationToken cancellationToken)
    {
        _accountAgnosticServiceMock
            .Setup(x => x.UpdateProfileAsync(
                command.AccountId!,
                command.FirstName!,
                command.LastName!,
                command.Image!.Id!,
                command.Image.Width,
                command.Image.Height,
                (ImageFormats)command.Image.ImageFormat,
                (FileStorageTypes)command.Image.FileStorageTypeId,
                cancellationToken))
            .ReturnsAsync(updatedAccount);
    }

    private void SetupCacheOperations(string accountId, CancellationToken cancellationToken)
    {
        _profileCacheManagerMock
            .Setup(x => x.ClearAsync(accountId))
            .Returns(Task.CompletedTask);

        _accountCacheManagerMock
            .Setup(x => x.ClearAsync())
            .Returns(Task.CompletedTask);

        _chatCacheManagerMock
            .Setup(x => x.ClearAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
    }

    private void SetupImageRemovalOperations(string accountId, List<string> accountIds, ImageServiceModel previousImage, RemoveImageRequest removeImageRequest, CancellationToken cancellationToken)
    {
        _chatAgnosticServiceMock
            .Setup(x => x.GetAccountIdsInIndividualChatsAsync(accountId, cancellationToken))
            .ReturnsAsync(accountIds);

        _mapperMock
            .Setup(x => x.Map<RemoveImageRequest>(previousImage))
            .Returns(removeImageRequest);

        _producerMock
            .Setup(x => x.PublishAsync(removeImageRequest, cancellationToken))
            .Returns(Task.CompletedTask);
    }

    private void SetupMapper(ProfileServiceModel serviceModel, ProfileDto dto)
    {
        _mapperMock
            .Setup(x => x.Map<ProfileDto>(serviceModel))
            .Returns(dto);
    }

    private void VerifyUpdateProfileWithoutImage(UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        _accountAgnosticServiceMock.Verify(
            x => x.UpdateProfileAsync(command.AccountId!, command.FirstName!, command.LastName!, cancellationToken),
            Times.Once);

        _accountAgnosticServiceMock.Verify(
            x => x.UpdateProfileAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<ImageFormats>(),
                It.IsAny<FileStorageTypes>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private void VerifyUpdateProfileWithImage(UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        _accountAgnosticServiceMock.Verify(
            x => x.UpdateProfileAsync(
                command.AccountId!,
                command.FirstName!,
                command.LastName!,
                command.Image!.Id!,
                command.Image.Width,
                command.Image.Height,
                (ImageFormats)command.Image.ImageFormat,
                (FileStorageTypes)command.Image.FileStorageTypeId,
                cancellationToken),
            Times.Once);

        _accountAgnosticServiceMock.Verify(
            x => x.UpdateProfileAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private void VerifyBasicCacheOperations(string accountId, CancellationToken cancellationToken)
    {
        _profileCacheManagerMock.Verify(
            x => x.ClearAsync(accountId),
            Times.Once);

        _accountCacheManagerMock.Verify(
            x => x.ClearAsync(),
            Times.Once);
    }

    private void VerifyImageRemovalOperations(string accountId, List<string> accountIds, ImageServiceModel previousImage, RemoveImageRequest removeImageRequest, CancellationToken cancellationToken)
    {
        _chatAgnosticServiceMock.Verify(
            x => x.GetAccountIdsInIndividualChatsAsync(accountId, cancellationToken),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<RemoveImageRequest>(previousImage),
            Times.Once);

        _producerMock.Verify(
            x => x.PublishAsync(removeImageRequest, cancellationToken),
            Times.Once);

        foreach (var id in accountIds)
        {
            _chatCacheManagerMock.Verify(
                x => x.ClearAsync(id),
                Times.Once);
        }
    }

    private void VerifyNoImageRemovalOperations(CancellationToken cancellationToken)
    {
        _chatAgnosticServiceMock.Verify(
            x => x.GetAccountIdsInIndividualChatsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _mapperMock.Verify(
            x => x.Map<RemoveImageRequest>(It.IsAny<ImageServiceModel>()),
            Times.Never);

        _producerMock.Verify(
            x => x.PublishAsync(It.IsAny<RemoveImageRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private void VerifyMapper(ProfileServiceModel serviceModel)
    {
        _mapperMock.Verify(
            x => x.Map<ProfileDto>(serviceModel),
            Times.Once);
    }
}
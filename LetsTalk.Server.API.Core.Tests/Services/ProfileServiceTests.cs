using AutoMapper;
using FluentAssertions;
using LetsTalk.Server.API.Core.Services;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using Moq;

namespace LetsTalk.Server.API.Core.Tests.Services;

[TestFixture]
public class ProfileServiceTests
{
    private Mock<IAccountAgnosticService> _accountAgnosticServiceMock;
    private Mock<IMapper> _mapperMock;
    private ProfileService _profileService;

    [SetUp]
    public void SetUp()
    {
        _accountAgnosticServiceMock = new Mock<IAccountAgnosticService>();
        _mapperMock = new Mock<IMapper>();
        _profileService = new ProfileService(_accountAgnosticServiceMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GetProfileAsync_ShouldReturnMappedProfile_WhenAccountExists()
    {
        // Arrange
        var accountId = "account123";
        var cancellationToken = CancellationToken.None;
        var serviceModel = new ProfileServiceModel
        {
            Id = accountId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };
        var expectedDto = new ProfileDto
        {
            Id = accountId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        _accountAgnosticServiceMock
            .Setup(x => x.GetByIdAsync(accountId, cancellationToken))
            .ReturnsAsync(serviceModel);
        _mapperMock
            .Setup(x => x.Map<ProfileDto>(serviceModel))
            .Returns(expectedDto);

        // Act
        var result = await _profileService.GetProfileAsync(accountId, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
        _accountAgnosticServiceMock.Verify(x => x.GetByIdAsync(accountId, cancellationToken), Times.Once);
        _mapperMock.Verify(x => x.Map<ProfileDto>(serviceModel), Times.Once);
    }

    [Test]
    public async Task GetProfileAsync_ShouldPassCancellationToken_ToAgnosticService()
    {
        // Arrange
        var accountId = "account123";
        var cancellationToken = new CancellationToken(true);
        var serviceModel = new ProfileServiceModel();
        var dto = new ProfileDto();

        _accountAgnosticServiceMock
            .Setup(x => x.GetByIdAsync(accountId, cancellationToken))
            .ReturnsAsync(serviceModel);
        _mapperMock
            .Setup(x => x.Map<ProfileDto>(serviceModel))
            .Returns(dto);

        // Act
        await _profileService.GetProfileAsync(accountId, cancellationToken);

        // Assert
        _accountAgnosticServiceMock.Verify(x => x.GetByIdAsync(accountId, cancellationToken), Times.Once);
    }

    [Test]
    public async Task GetProfileAsync_ShouldMapServiceModelToDto()
    {
        // Arrange
        var accountId = "account456";
        var cancellationToken = CancellationToken.None;
        var serviceModel = new ProfileServiceModel
        {
            Id = accountId,
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            PhotoUrl = "https://example.com/photo.jpg"
        };
        var expectedDto = new ProfileDto
        {
            Id = accountId,
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            PhotoUrl = "https://example.com/photo.jpg"
        };

        _accountAgnosticServiceMock
            .Setup(x => x.GetByIdAsync(accountId, cancellationToken))
            .ReturnsAsync(serviceModel);
        _mapperMock
            .Setup(x => x.Map<ProfileDto>(serviceModel))
            .Returns(expectedDto);

        // Act
        var result = await _profileService.GetProfileAsync(accountId, cancellationToken);

        // Assert
        result.Should().Be(expectedDto);
        _mapperMock.Verify(x => x.Map<ProfileDto>(serviceModel), Times.Once);
    }
}
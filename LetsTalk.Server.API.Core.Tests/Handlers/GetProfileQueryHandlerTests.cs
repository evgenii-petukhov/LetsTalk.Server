using AutoMapper;
using FluentAssertions;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Features.Profile.Queries.GetProfile;
using LetsTalk.Server.Dto.Models;
using MediatR;
using Moq;
using NUnit.Framework;

namespace LetsTalk.Server.API.Core.Tests.Handlers;

[TestFixture]
public class GetProfileQueryHandlerTests
{
    private Mock<IMapper> _mapperMock;
    private Mock<IProfileService> _profileServiceMock;
    private GetProfileQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _mapperMock = new Mock<IMapper>();
        _profileServiceMock = new Mock<IProfileService>();

        _handler = new GetProfileQueryHandler(
            _mapperMock.Object,
            _profileServiceMock.Object);
    }

    [Test]
    public async Task Handle_WhenProfileExists_ShouldReturnMappedProfile()
    {
        // Arrange
        var query = new GetProfileQuery("account-123");
        var cancellationToken = CancellationToken.None;

        var serviceProfile = new ProfileDto
        {
            Id = "account-123",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PhotoUrl = "https://example.com/photo.jpg"
        };

        var mappedProfile = new ProfileDto
        {
            Id = "account-123",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PhotoUrl = "https://example.com/photo.jpg"
        };

        _profileServiceMock
            .Setup(x => x.GetProfileAsync("account-123", cancellationToken))
            .ReturnsAsync(serviceProfile);

        _mapperMock
            .Setup(x => x.Map<ProfileDto>(serviceProfile))
            .Returns(mappedProfile);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(mappedProfile);

        _profileServiceMock.Verify(
            x => x.GetProfileAsync("account-123", cancellationToken),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<ProfileDto>(serviceProfile),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenProfileHasImage_ShouldReturnProfileWithImage()
    {
        // Arrange
        var query = new GetProfileQuery("account-456");
        var cancellationToken = CancellationToken.None;

        var imageDto = new ImageDto
        {
            Id = "image-123",
            FileStorageTypeId = 1
        };

        var serviceProfile = new ProfileDto
        {
            Id = "account-456",
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            Image = imageDto
        };

        var mappedProfile = new ProfileDto
        {
            Id = "account-456",
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            Image = imageDto
        };

        _profileServiceMock
            .Setup(x => x.GetProfileAsync("account-456", cancellationToken))
            .ReturnsAsync(serviceProfile);

        _mapperMock
            .Setup(x => x.Map<ProfileDto>(serviceProfile))
            .Returns(mappedProfile);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(mappedProfile);
        result.Image.Should().BeEquivalentTo(imageDto);

        _profileServiceMock.Verify(
            x => x.GetProfileAsync("account-456", cancellationToken),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<ProfileDto>(serviceProfile),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenProfileHasNullProperties_ShouldReturnProfileWithNullValues()
    {
        // Arrange
        var query = new GetProfileQuery("account-789");
        var cancellationToken = CancellationToken.None;

        var serviceProfile = new ProfileDto
        {
            Id = "account-789",
            FirstName = null,
            LastName = null,
            Email = null,
            PhotoUrl = null,
            Image = null
        };

        var mappedProfile = new ProfileDto
        {
            Id = "account-789",
            FirstName = null,
            LastName = null,
            Email = null,
            PhotoUrl = null,
            Image = null
        };

        _profileServiceMock
            .Setup(x => x.GetProfileAsync("account-789", cancellationToken))
            .ReturnsAsync(serviceProfile);

        _mapperMock
            .Setup(x => x.Map<ProfileDto>(serviceProfile))
            .Returns(mappedProfile);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(mappedProfile);
        result.FirstName.Should().BeNull();
        result.LastName.Should().BeNull();
        result.Email.Should().BeNull();
        result.PhotoUrl.Should().BeNull();
        result.Image.Should().BeNull();

        _profileServiceMock.Verify(
            x => x.GetProfileAsync("account-789", cancellationToken),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<ProfileDto>(serviceProfile),
            Times.Once);
    }

    [Test]
    public void Handle_WhenProfileServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var query = new GetProfileQuery("account-123");
        var cancellationToken = CancellationToken.None;

        _profileServiceMock
            .Setup(x => x.GetProfileAsync("account-123", cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Profile not found"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, cancellationToken));

        _profileServiceMock.Verify(
            x => x.GetProfileAsync("account-123", cancellationToken),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<ProfileDto>(It.IsAny<ProfileDto>()),
            Times.Never);
    }

    [Test]
    public void Handle_WhenMapperThrowsException_ShouldPropagateException()
    {
        // Arrange
        var query = new GetProfileQuery("account-123");
        var cancellationToken = CancellationToken.None;

        var serviceProfile = new ProfileDto { Id = "account-123" };

        _profileServiceMock
            .Setup(x => x.GetProfileAsync("account-123", cancellationToken))
            .ReturnsAsync(serviceProfile);

        _mapperMock
            .Setup(x => x.Map<ProfileDto>(serviceProfile))
            .Throws(new InvalidOperationException("Mapping failed"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, cancellationToken));

        _profileServiceMock.Verify(
            x => x.GetProfileAsync("account-123", cancellationToken),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<ProfileDto>(serviceProfile),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenCancellationTokenProvided_ShouldPassItToService()
    {
        // Arrange
        var query = new GetProfileQuery("account-123");
        var cancellationToken = new CancellationToken(false);

        var serviceProfile = new ProfileDto { Id = "account-123" };
        var mappedProfile = new ProfileDto { Id = "account-123" };

        _profileServiceMock
            .Setup(x => x.GetProfileAsync("account-123", cancellationToken))
            .ReturnsAsync(serviceProfile);

        _mapperMock
            .Setup(x => x.Map<ProfileDto>(serviceProfile))
            .Returns(mappedProfile);

        // Act
        await _handler.Handle(query, cancellationToken);

        // Assert - Verification is done in the setup methods that check cancellationToken
        _profileServiceMock.Verify(
            x => x.GetProfileAsync("account-123", cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenNullAccountId_ShouldPassNullToService()
    {
        // Arrange
        var query = new GetProfileQuery(null!);
        var cancellationToken = CancellationToken.None;

        var serviceProfile = new ProfileDto { Id = null };
        var mappedProfile = new ProfileDto { Id = null };

        _profileServiceMock
            .Setup(x => x.GetProfileAsync(null!, cancellationToken))
            .ReturnsAsync(serviceProfile);

        _mapperMock
            .Setup(x => x.Map<ProfileDto>(serviceProfile))
            .Returns(mappedProfile);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(mappedProfile);

        _profileServiceMock.Verify(
            x => x.GetProfileAsync(null!, cancellationToken),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<ProfileDto>(serviceProfile),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenEmptyAccountId_ShouldPassEmptyStringToService()
    {
        // Arrange
        var query = new GetProfileQuery("");
        var cancellationToken = CancellationToken.None;

        var serviceProfile = new ProfileDto { Id = "" };
        var mappedProfile = new ProfileDto { Id = "" };

        _profileServiceMock
            .Setup(x => x.GetProfileAsync("", cancellationToken))
            .ReturnsAsync(serviceProfile);

        _mapperMock
            .Setup(x => x.Map<ProfileDto>(serviceProfile))
            .Returns(mappedProfile);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(mappedProfile);

        _profileServiceMock.Verify(
            x => x.GetProfileAsync("", cancellationToken),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<ProfileDto>(serviceProfile),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenSpecialCharactersInAccountId_ShouldHandleCorrectly()
    {
        // Arrange
        var accountId = "account-!@#$%^&*()";
        var query = new GetProfileQuery(accountId);
        var cancellationToken = CancellationToken.None;

        var serviceProfile = new ProfileDto
        {
            Id = accountId,
            FirstName = "John-O'Connor",
            LastName = "Smith & Co.",
            Email = "john+test@example.com"
        };

        var mappedProfile = new ProfileDto
        {
            Id = accountId,
            FirstName = "John-O'Connor",
            LastName = "Smith & Co.",
            Email = "john+test@example.com"
        };

        _profileServiceMock
            .Setup(x => x.GetProfileAsync(accountId, cancellationToken))
            .ReturnsAsync(serviceProfile);

        _mapperMock
            .Setup(x => x.Map<ProfileDto>(serviceProfile))
            .Returns(mappedProfile);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(mappedProfile);
        result.FirstName.Should().Be("John-O'Connor");
        result.LastName.Should().Be("Smith & Co.");
        result.Email.Should().Be("john+test@example.com");

        _profileServiceMock.Verify(
            x => x.GetProfileAsync(accountId, cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenUnicodeCharactersInAccountId_ShouldHandleCorrectly()
    {
        // Arrange
        var accountId = "账户-123";
        var query = new GetProfileQuery(accountId);
        var cancellationToken = CancellationToken.None;

        var serviceProfile = new ProfileDto
        {
            Id = accountId,
            FirstName = "张三",
            LastName = "李四 🌟",
            Email = "测试@example.com"
        };

        var mappedProfile = new ProfileDto
        {
            Id = accountId,
            FirstName = "张三",
            LastName = "李四 🌟",
            Email = "测试@example.com"
        };

        _profileServiceMock
            .Setup(x => x.GetProfileAsync(accountId, cancellationToken))
            .ReturnsAsync(serviceProfile);

        _mapperMock
            .Setup(x => x.Map<ProfileDto>(serviceProfile))
            .Returns(mappedProfile);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(mappedProfile);
        result.FirstName.Should().Be("张三");
        result.LastName.Should().Be("李四 🌟");
        result.Email.Should().Be("测试@example.com");

        _profileServiceMock.Verify(
            x => x.GetProfileAsync(accountId, cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenLongAccountId_ShouldHandleCorrectly()
    {
        // Arrange
        var accountId = new string('a', 1000); // Very long account ID
        var query = new GetProfileQuery(accountId);
        var cancellationToken = CancellationToken.None;

        var serviceProfile = new ProfileDto
        {
            Id = accountId,
            FirstName = "John",
            LastName = "Doe"
        };

        var mappedProfile = new ProfileDto
        {
            Id = accountId,
            FirstName = "John",
            LastName = "Doe"
        };

        _profileServiceMock
            .Setup(x => x.GetProfileAsync(accountId, cancellationToken))
            .ReturnsAsync(serviceProfile);

        _mapperMock
            .Setup(x => x.Map<ProfileDto>(serviceProfile))
            .Returns(mappedProfile);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(mappedProfile);
        result.Id.Should().Be(accountId);
        result.Id.Should().HaveLength(1000);

        _profileServiceMock.Verify(
            x => x.GetProfileAsync(accountId, cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenProfileServiceReturnsNull_ShouldReturnMappedNull()
    {
        // Arrange
        var query = new GetProfileQuery("account-123");
        var cancellationToken = CancellationToken.None;

        _profileServiceMock
            .Setup(x => x.GetProfileAsync("account-123", cancellationToken))
            .ReturnsAsync((ProfileDto?)null);

        _mapperMock
            .Setup(x => x.Map<ProfileDto>((ProfileDto?)null))
            .Returns((ProfileDto?)null);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().BeNull();

        _profileServiceMock.Verify(
            x => x.GetProfileAsync("account-123", cancellationToken),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<ProfileDto>((ProfileDto?)null),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenProfileHasComplexImageData_ShouldReturnCompleteProfile()
    {
        // Arrange
        var query = new GetProfileQuery("account-complex");
        var cancellationToken = CancellationToken.None;

        var complexImage = new ImageDto
        {
            Id = "image-complex-123",
            FileStorageTypeId = 2
        };

        var serviceProfile = new ProfileDto
        {
            Id = "account-complex",
            FirstName = "María José",
            LastName = "García-López",
            Email = "maria.jose@empresa-española.com",
            PhotoUrl = "https://cdn.example.com/profiles/maría-josé-garcía-lópez.jpg",
            Image = complexImage
        };

        var mappedProfile = new ProfileDto
        {
            Id = "account-complex",
            FirstName = "María José",
            LastName = "García-López",
            Email = "maria.jose@empresa-española.com",
            PhotoUrl = "https://cdn.example.com/profiles/maría-josé-garcía-lópez.jpg",
            Image = complexImage
        };

        _profileServiceMock
            .Setup(x => x.GetProfileAsync("account-complex", cancellationToken))
            .ReturnsAsync(serviceProfile);

        _mapperMock
            .Setup(x => x.Map<ProfileDto>(serviceProfile))
            .Returns(mappedProfile);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(mappedProfile);
        result.Image.Should().BeEquivalentTo(complexImage);
        result.Image!.Id.Should().Be("image-complex-123");
        result.Image.FileStorageTypeId.Should().Be(2);

        _profileServiceMock.Verify(
            x => x.GetProfileAsync("account-complex", cancellationToken),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<ProfileDto>(serviceProfile),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenMultipleCallsWithSameId_ShouldCallServiceEachTime()
    {
        // Arrange
        var query = new GetProfileQuery("account-123");
        var cancellationToken = CancellationToken.None;

        var serviceProfile = new ProfileDto { Id = "account-123", FirstName = "John" };
        var mappedProfile = new ProfileDto { Id = "account-123", FirstName = "John" };

        _profileServiceMock
            .Setup(x => x.GetProfileAsync("account-123", cancellationToken))
            .ReturnsAsync(serviceProfile);

        _mapperMock
            .Setup(x => x.Map<ProfileDto>(serviceProfile))
            .Returns(mappedProfile);

        // Act
        var result1 = await _handler.Handle(query, cancellationToken);
        var result2 = await _handler.Handle(query, cancellationToken);
        var result3 = await _handler.Handle(query, cancellationToken);

        // Assert
        result1.Should().BeEquivalentTo(mappedProfile);
        result2.Should().BeEquivalentTo(mappedProfile);
        result3.Should().BeEquivalentTo(mappedProfile);

        _profileServiceMock.Verify(
            x => x.GetProfileAsync("account-123", cancellationToken),
            Times.Exactly(3));

        _mapperMock.Verify(
            x => x.Map<ProfileDto>(serviceProfile),
            Times.Exactly(3));
    }

    [Test]
    public async Task Handle_WhenDifferentAccountIds_ShouldCallServiceWithCorrectIds()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var accountIds = new[] { "account-1", "account-2", "account-3" };
        var queries = accountIds.Select(id => new GetProfileQuery(id)).ToArray();

        var serviceProfiles = accountIds.Select(id => new ProfileDto { Id = id, FirstName = $"User{id}" }).ToArray();
        var mappedProfiles = accountIds.Select(id => new ProfileDto { Id = id, FirstName = $"User{id}" }).ToArray();

        for (int i = 0; i < accountIds.Length; i++)
        {
            _profileServiceMock
                .Setup(x => x.GetProfileAsync(accountIds[i], cancellationToken))
                .ReturnsAsync(serviceProfiles[i]);

            _mapperMock
                .Setup(x => x.Map<ProfileDto>(serviceProfiles[i]))
                .Returns(mappedProfiles[i]);
        }

        // Act
        var results = new List<ProfileDto>();
        foreach (var query in queries)
        {
            results.Add(await _handler.Handle(query, cancellationToken));
        }

        // Assert
        for (int i = 0; i < accountIds.Length; i++)
        {
            results[i].Should().BeEquivalentTo(mappedProfiles[i]);
            results[i].Id.Should().Be(accountIds[i]);
            results[i].FirstName.Should().Be($"User{accountIds[i]}");

            _profileServiceMock.Verify(
                x => x.GetProfileAsync(accountIds[i], cancellationToken),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<ProfileDto>(serviceProfiles[i]),
                Times.Once);
        }
    }
}
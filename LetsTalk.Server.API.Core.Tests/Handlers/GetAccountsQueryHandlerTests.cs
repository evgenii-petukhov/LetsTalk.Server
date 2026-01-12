using FluentAssertions;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Features.Account.Queries.GetAccounts;
using LetsTalk.Server.Dto.Models;
using Moq;

namespace LetsTalk.Server.API.Core.Tests.Handlers;

[TestFixture]
public class GetAccountsQueryHandlerTests
{
    private Mock<IAccountService> _accountServiceMock;
    private GetAccountsQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _accountServiceMock = new Mock<IAccountService>();
        _handler = new GetAccountsQueryHandler(_accountServiceMock.Object);
    }

    [Test]
    public async Task Handle_WhenAccountsExist_ShouldReturnAccountsExcludingRequesterId()
    {
        // Arrange
        var requesterId = "requester-123";
        var query = new GetAccountsQuery(requesterId);
        var cancellationToken = CancellationToken.None;

        var accounts = new List<AccountDto>
        {
            new() { Id = "user-1", FirstName = "John", LastName = "Doe", AccountTypeId = 1 },
            new() { Id = requesterId, FirstName = "Requester", LastName = "User", AccountTypeId = 1 },
            new() { Id = "user-2", FirstName = "Jane", LastName = "Smith", AccountTypeId = 2 }
        };

        _accountServiceMock
            .Setup(x => x.GetAccountsAsync(cancellationToken))
            .ReturnsAsync(accounts);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().NotContain(account => account.Id == requesterId);
        result.Should().Contain(account => account.Id == "user-1");
        result.Should().Contain(account => account.Id == "user-2");
    }

    [Test]
    public async Task Handle_WhenNoAccountsExist_ShouldReturnEmptyList()
    {
        // Arrange
        var requesterId = "requester-123";
        var query = new GetAccountsQuery(requesterId);
        var cancellationToken = CancellationToken.None;

        var accounts = new List<AccountDto>();

        _accountServiceMock
            .Setup(x => x.GetAccountsAsync(cancellationToken))
            .ReturnsAsync(accounts);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Test]
    public async Task Handle_WhenOnlyRequesterAccountExists_ShouldReturnEmptyList()
    {
        // Arrange
        var requesterId = "requester-123";
        var query = new GetAccountsQuery(requesterId);
        var cancellationToken = CancellationToken.None;

        var accounts = new List<AccountDto>
        {
            new() { Id = requesterId, FirstName = "Requester", LastName = "User", AccountTypeId = 1 }
        };

        _accountServiceMock
            .Setup(x => x.GetAccountsAsync(cancellationToken))
            .ReturnsAsync(accounts);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Test]
    public async Task Handle_WhenRequesterIdNotInAccountsList_ShouldReturnAllAccounts()
    {
        // Arrange
        var requesterId = "non-existent-user";
        var query = new GetAccountsQuery(requesterId);
        var cancellationToken = CancellationToken.None;

        var accounts = new List<AccountDto>
        {
            new() { Id = "user-1", FirstName = "John", LastName = "Doe", AccountTypeId = 1 },
            new() { Id = "user-2", FirstName = "Jane", LastName = "Smith", AccountTypeId = 2 }
        };

        _accountServiceMock
            .Setup(x => x.GetAccountsAsync(cancellationToken))
            .ReturnsAsync(accounts);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(account => account.Id == "user-1");
        result.Should().Contain(account => account.Id == "user-2");
    }

    [Test]
    public async Task Handle_WhenAccountsHaveComplexData_ShouldPreserveAllProperties()
    {
        // Arrange
        var requesterId = "requester-123";
        var query = new GetAccountsQuery(requesterId);
        var cancellationToken = CancellationToken.None;

        var imageDto = new ImageDto { Id = "image-1", FileStorageTypeId = 1 };
        var accounts = new List<AccountDto>
        {
            new()
            {
                Id = "user-1",
                FirstName = "John",
                LastName = "Doe",
                AccountTypeId = 1,
                PhotoUrl = "https://example.com/photo1.jpg",
                Image = imageDto
            },
            new()
            {
                Id = requesterId,
                FirstName = "Requester",
                LastName = "User",
                AccountTypeId = 2,
                PhotoUrl = "https://example.com/requester.jpg",
                Image = null
            }
        };

        _accountServiceMock
            .Setup(x => x.GetAccountsAsync(cancellationToken))
            .ReturnsAsync(accounts);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        
        var returnedAccount = result.First();
        returnedAccount.Id.Should().Be("user-1");
        returnedAccount.FirstName.Should().Be("John");
        returnedAccount.LastName.Should().Be("Doe");
        returnedAccount.AccountTypeId.Should().Be(1);
        returnedAccount.PhotoUrl.Should().Be("https://example.com/photo1.jpg");
        returnedAccount.Image.Should().NotBeNull();
        returnedAccount.Image!.Id.Should().Be("image-1");
        returnedAccount.Image.FileStorageTypeId.Should().Be(1);
    }

    [Test]
    public async Task Handle_WhenCancellationTokenProvided_ShouldPassItToService()
    {
        // Arrange
        var requesterId = "requester-123";
        var query = new GetAccountsQuery(requesterId);
        var cancellationToken = new CancellationToken(true);

        _accountServiceMock
            .Setup(x => x.GetAccountsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AccountDto>());

        // Act
        await _handler.Handle(query, cancellationToken);

        // Assert
        _accountServiceMock.Verify(
            x => x.GetAccountsAsync(cancellationToken),
            Times.Once);
    }

    [Test]
    public void Handle_WhenServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var requesterId = "requester-123";
        var query = new GetAccountsQuery(requesterId);
        var cancellationToken = CancellationToken.None;

        _accountServiceMock
            .Setup(x => x.GetAccountsAsync(cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Service error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(query, cancellationToken));
    }

    [Test]
    public async Task Handle_WhenAccountsHaveNullIds_ShouldHandleGracefully()
    {
        // Arrange
        var requesterId = "requester-123";
        var query = new GetAccountsQuery(requesterId);
        var cancellationToken = CancellationToken.None;

        var accounts = new List<AccountDto>
        {
            new() { Id = null, FirstName = "John", LastName = "Doe", AccountTypeId = 1 },
            new() { Id = requesterId, FirstName = "Requester", LastName = "User", AccountTypeId = 1 },
            new() { Id = "user-2", FirstName = "Jane", LastName = "Smith", AccountTypeId = 2 }
        };

        _accountServiceMock
            .Setup(x => x.GetAccountsAsync(cancellationToken))
            .ReturnsAsync(accounts);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().NotContain(account => account.Id == requesterId);
        result.Should().Contain(account => account.Id == null);
        result.Should().Contain(account => account.Id == "user-2");
    }

    [Test]
    public async Task Handle_WhenRequesterIdIsNull_ShouldReturnAllAccountsExceptNull()
    {
        // Arrange
        var query = new GetAccountsQuery(null!);
        var cancellationToken = CancellationToken.None;

        var accounts = new List<AccountDto>
        {
            new() { Id = null, FirstName = "John", LastName = "Doe", AccountTypeId = 1 },
            new() { Id = "user-1", FirstName = "Jane", LastName = "Smith", AccountTypeId = 2 }
        };

        _accountServiceMock
            .Setup(x => x.GetAccountsAsync(cancellationToken))
            .ReturnsAsync(accounts);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().Contain(account => account.Id == "user-1");
        result.Should().NotContain(account => account.Id == null);
    }

}
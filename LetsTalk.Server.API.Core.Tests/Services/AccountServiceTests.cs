using AutoMapper;
using FluentAssertions;
using LetsTalk.Server.API.Core.Services;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Models;
using Moq;

namespace LetsTalk.Server.API.Core.Tests.Services;

[TestFixture]
public class AccountServiceTests
{
    private Mock<IAccountAgnosticService> _accountAgnosticServiceMock;
    private Mock<IMapper> _mapperMock;
    private AccountService _accountService;

    [SetUp]
    public void SetUp()
    {
        _accountAgnosticServiceMock = new Mock<IAccountAgnosticService>();
        _mapperMock = new Mock<IMapper>();
        _accountService = new AccountService(_accountAgnosticServiceMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GetAccountsAsync_ShouldReturnMappedAccounts_WhenAccountsExist()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var serviceModels = new List<AccountServiceModel>
        {
            new() { Id = "1", FirstName = "John", LastName = "Doe" },
            new() { Id = "2", FirstName = "Jane", LastName = "Smith" }
        };
        var expectedDtos = new List<AccountDto>
        {
            new() { Id = "1", FirstName = "John", LastName = "Doe" },
            new() { Id = "2", FirstName = "Jane", LastName = "Smith" }
        };

        _accountAgnosticServiceMock
            .Setup(x => x.GetAccountsAsync(cancellationToken))
            .ReturnsAsync(serviceModels);
        _mapperMock
            .Setup(x => x.Map<List<AccountDto>>(serviceModels))
            .Returns(expectedDtos);

        // Act
        var result = await _accountService.GetAccountsAsync(cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedDtos);
        _accountAgnosticServiceMock.Verify(x => x.GetAccountsAsync(cancellationToken), Times.Once);
        _mapperMock.Verify(x => x.Map<List<AccountDto>>(serviceModels), Times.Once);
    }

    [Test]
    public async Task GetAccountsAsync_ShouldReturnEmptyList_WhenNoAccountsExist()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var emptyServiceModels = new List<AccountServiceModel>();
        var emptyDtos = new List<AccountDto>();

        _accountAgnosticServiceMock
            .Setup(x => x.GetAccountsAsync(cancellationToken))
            .ReturnsAsync(emptyServiceModels);
        _mapperMock
            .Setup(x => x.Map<List<AccountDto>>(emptyServiceModels))
            .Returns(emptyDtos);

        // Act
        var result = await _accountService.GetAccountsAsync(cancellationToken);

        // Assert
        result.Should().BeEmpty();
        _accountAgnosticServiceMock.Verify(x => x.GetAccountsAsync(cancellationToken), Times.Once);
        _mapperMock.Verify(x => x.Map<List<AccountDto>>(emptyServiceModels), Times.Once);
    }

    [Test]
    public async Task GetAccountsAsync_ShouldPassCancellationToken_ToAgnosticService()
    {
        // Arrange
        var cancellationToken = new CancellationToken(true);
        var serviceModels = new List<AccountServiceModel>();
        var dtos = new List<AccountDto>();

        _accountAgnosticServiceMock
            .Setup(x => x.GetAccountsAsync(cancellationToken))
            .ReturnsAsync(serviceModels);
        _mapperMock
            .Setup(x => x.Map<List<AccountDto>>(serviceModels))
            .Returns(dtos);

        // Act
        await _accountService.GetAccountsAsync(cancellationToken);

        // Assert
        _accountAgnosticServiceMock.Verify(x => x.GetAccountsAsync(cancellationToken), Times.Once);
    }
}
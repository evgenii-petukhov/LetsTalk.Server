using FluentAssertions;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.API.Core.Features.Chat.Commands.CreateIndividualChat;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using Moq;

namespace LetsTalk.Server.API.Core.Tests.Validators;

[TestFixture]
public class CreateIndividualChatCommandValidatorTests
{
    private const string SampleInvitingAccountId = "1";
    private const string SampleAccountId = "2";
    private const string SampleAccountIdString = "6637c3b9ad6508214b2455f3";

    private CreateIndividualChatCommandValidator _validator;
    private Mock<IAccountAgnosticService> _mockAccountAgnosticService;

    [SetUp]
    public void SetUp()
    {
        _mockAccountAgnosticService = new Mock<IAccountAgnosticService>();
        _validator = new(_mockAccountAgnosticService.Object);
    }

    [Test]
    [TestCase(SampleAccountId)]
    [TestCase(SampleAccountIdString)]
    public async Task ValidateAsync_When_AccountIdIsInvalid_AccountDoesNotExist_ShouldContainValidationErrors(string accountId)
    {
        // Arrange
        var request = new CreateIndividualChatCommand(SampleInvitingAccountId, accountId);
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(
        [
            $"Account with 'Account Id' = '{accountId}' must exist"
        ]);
        _mockAccountAgnosticService.Verify(x => x.IsAccountIdValidAsync(accountId, cancellationToken), Times.Once);
    }

    [Test]
    [TestCase(SampleAccountId)]
    [TestCase(SampleAccountIdString)]
    public async Task ValidateAsync_When_AccountIdIsInvalid_AccountExists_ShouldBeValid(string accountId)
    {
        // Arrange
        var request = new CreateIndividualChatCommand(SampleInvitingAccountId, accountId);
        var cancellationToken = new CancellationToken();

        _mockAccountAgnosticService
            .Setup(x => x.IsAccountIdValidAsync(accountId, cancellationToken))
            .ReturnsAsync(true);

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        _mockAccountAgnosticService.Verify(x => x.IsAccountIdValidAsync(accountId, cancellationToken), Times.Once);
    }
}
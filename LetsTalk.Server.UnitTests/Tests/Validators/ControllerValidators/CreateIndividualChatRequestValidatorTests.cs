using FluentAssertions;
using LetsTalk.Server.API.Models.Chat;
using LetsTalk.Server.API.Validators;

namespace LetsTalk.Server.UnitTests.Tests.Validators.ControllerValidators;

[TestFixture]
public class CreateIndividualChatRequestValidatorTests
{
    private CreateIndividualChatRequestValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new();
    }

    [Test]
    public async Task ValidateAsync_When_AccountIdIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateIndividualChatRequest();
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Account Id is required",
            "Account Id cannot be empty"
        });
    }

    [Test]
    [TestCase("")]
    [TestCase(" ")]
    public async Task ValidateAsync_When_AccountIdIsEmpty_ShouldContainValidationErrors(string accountId)
    {
        // Arrange
        var request = new CreateIndividualChatRequest
        {
            AccountId = accountId
        };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Account Id cannot be empty"
        });
    }

    [Test]
    [TestCase("0")]
    [TestCase("-1")]
    public async Task ValidateAsync_When_AccountIdIsInvalidInteger_ShouldContainValidationErrors(string accountId)
    {
        // Arrange
        var request = new CreateIndividualChatRequest
        {
            AccountId = accountId
        };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Account Id must be greater than 0, if integer"
        });
    }

    [Test]
    [TestCase("1")]
    [TestCase("abc")]
    public async Task ValidateAsync_When_AccountIdIsValid_ShouldBeValid(string accountId)
    {
        // Arrange
        var request = new CreateIndividualChatRequest
        {
            AccountId = accountId
        };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }
}

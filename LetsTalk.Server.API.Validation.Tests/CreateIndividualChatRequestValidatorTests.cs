using FluentAssertions;
using LetsTalk.Server.API.Models.Chat;

namespace LetsTalk.Server.API.Validation.Tests;

[TestFixture]
public class CreateIndividualChatRequestValidatorTests
{
    private CreateIndividualChatRequestValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new CreateIndividualChatRequestValidator();
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
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo([
            "Account Id is required",
            "Account Id cannot be empty"
        ]);
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
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo([
            "Account Id cannot be empty"
        ]);
    }

    [Test]
    [TestCase("1")]
    [TestCase("123")]
    [TestCase("999")]
    public async Task ValidateAsync_When_AccountIdIsValidPositiveInteger_ShouldBeValid(string accountId)
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

    [Test]
    [TestCase("0")]
    [TestCase("-1")]
    [TestCase("-123")]
    public async Task ValidateAsync_When_AccountIdIsIntegerButNotPositive_ShouldContainValidationErrors(string accountId)
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
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo([
            "Account Id must be greater than 0, if integer"
        ]);
    }

    [Test]
    [TestCase("abc")]
    [TestCase("user123")]
    [TestCase("12.34")]
    public async Task ValidateAsync_When_AccountIdIsNotInteger_ShouldBeValid(string accountId)
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
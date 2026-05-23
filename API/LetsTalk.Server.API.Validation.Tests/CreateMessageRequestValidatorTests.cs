using FluentAssertions;
using LetsTalk.Server.API.Models.Message;
using LetsTalk.Server.SignPackage.Abstractions;
using Moq;

namespace LetsTalk.Server.API.Validation.Tests;

[TestFixture]
public class CreateMessageRequestValidatorTests
{
    private Mock<ISignPackageService> _signPackageServiceMock;
    private CreateMessageRequestValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _signPackageServiceMock = new Mock<ISignPackageService>();
        _validator = new CreateMessageRequestValidator(_signPackageServiceMock.Object);
    }

    [Test]
    public async Task ValidateAsync_When_ChatIdIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageRequest { Text = "test" };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo([
            "Chat Id is required",
            "Chat Id cannot be empty"
        ]);
    }

    [Test]
    [TestCase("")]
    [TestCase(" ")]
    public async Task ValidateAsync_When_ChatIdIsEmpty_ShouldContainValidationErrors(string chatId)
    {
        // Arrange
        var request = new CreateMessageRequest { ChatId = chatId, Text = "test" };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo([
            "Chat Id cannot be empty"
        ]);
    }

    [Test]
    [TestCase("0")]
    [TestCase("-1")]
    [TestCase("-123")]
    public async Task ValidateAsync_When_ChatIdIsIntegerButNotPositive_ShouldContainValidationErrors(string chatId)
    {
        // Arrange
        var request = new CreateMessageRequest { ChatId = chatId, Text = "test" };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo([
            "Chat Id must be greater than 0, if integer"
        ]);
    }

    [Test]
    public async Task ValidateAsync_When_TextAndImageAreEmpty_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new CreateMessageRequest { ChatId = "1" };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo([
            "Text and ImageId both cannot be empty"
        ]);
    }

    [Test]
    public async Task ValidateAsync_When_ImageSignatureIsInvalid_ShouldContainValidationErrors()
    {
        // Arrange
        var image = new ImageRequestModel { Id = "test" };
        var request = new CreateMessageRequest { ChatId = "1", Image = image };
        var cancellationToken = new CancellationToken();

        _signPackageServiceMock.Setup(x => x.Validate(image)).Returns(false);

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo([
            "Image signature is invalid"
        ]);
    }

    [Test]
    [TestCase("1")]
    [TestCase("123")]
    public async Task ValidateAsync_When_ValidWithText_ShouldBeValid(string chatId)
    {
        // Arrange
        var request = new CreateMessageRequest { ChatId = chatId, Text = "test message" };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }

    [Test]
    public async Task ValidateAsync_When_ValidWithImage_ShouldBeValid()
    {
        // Arrange
        var image = new ImageRequestModel { Id = "test" };
        var request = new CreateMessageRequest { ChatId = "1", Image = image };
        var cancellationToken = new CancellationToken();

        _signPackageServiceMock.Setup(x => x.Validate(image)).Returns(true);

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }

    [Test]
    [TestCase("abc")]
    [TestCase("user123")]
    public async Task ValidateAsync_When_ChatIdIsNotInteger_ShouldBeValid(string chatId)
    {
        // Arrange
        var request = new CreateMessageRequest { ChatId = chatId, Text = "test" };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }
}
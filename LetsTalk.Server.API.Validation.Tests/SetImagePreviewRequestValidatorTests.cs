using FluentAssertions;
using LetsTalk.Server.API.Models.Message;
using LetsTalk.Server.SignPackage.Abstractions;
using Moq;

namespace LetsTalk.Server.API.Validation.Tests;

[TestFixture]
public class SetImagePreviewRequestValidatorTests
{
    private Mock<ISignPackageService> _signPackageServiceMock;
    private SetImagePreviewRequestValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _signPackageServiceMock = new Mock<ISignPackageService>();
        _validator = new SetImagePreviewRequestValidator(_signPackageServiceMock.Object);
    }

    [Test]
    public async Task ValidateAsync_When_SignatureIsInvalid_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new SetImagePreviewRequest { MessageId = "1", ChatId = "1" };
        var cancellationToken = new CancellationToken();

        _signPackageServiceMock.Setup(x => x.Validate(request)).Returns(false);

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
    public async Task ValidateAsync_When_SignatureIsValid_ShouldBeValid()
    {
        // Arrange
        var request = new SetImagePreviewRequest { MessageId = "1", ChatId = "1" };
        var cancellationToken = new CancellationToken();

        _signPackageServiceMock.Setup(x => x.Validate(request)).Returns(true);

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }
}
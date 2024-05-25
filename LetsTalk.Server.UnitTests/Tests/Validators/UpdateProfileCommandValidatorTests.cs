using FluentAssertions;
using LetsTalk.Server.API.Models.Message;
using LetsTalk.Server.API.Core.Features.Profile.Commands.UpdateProfile;
using LetsTalk.Server.SignPackage.Abstractions;
using Moq;

namespace LetsTalk.Server.UnitTests.Tests.Validators;

#pragma warning disable CA1861 // Avoid constant arrays as arguments

[TestFixture]
public class UpdateProfileCommandValidatorTests
{
    private UpdateProfileCommandValidator _validator;
    private Mock<ISignPackageService> _mockSignPackageService;

    [SetUp]
    public void SetUp()
    {
        _mockSignPackageService = new Mock<ISignPackageService>();
        _validator = new(_mockSignPackageService.Object);
    }

    [Test]
    public async Task ValidateAsync_When_ModelIsEmpty_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new UpdateProfileCommand();
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "First Name is required",
            "First Name cannot be empty",
            "Last Name is required",
            "Last Name cannot be empty",
        });
    }

    [Test]
    public async Task ValidateAsync_When_FirstNameIsEmpty_LastNameIsEmpty_ImageIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            FirstName = string.Empty,
            LastName = string.Empty
        };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "First Name cannot be empty",
            "Last Name cannot be empty",
        });
    }

    [Test]
    public async Task ValidateAsync_When_FirstNameIsNotEmpty_LastNameIsNotEmpty_ImageIsNotNull_SignatureIsInvalid_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            FirstName = "test",
            LastName = "test",
            Image = new ImageRequestModel
            {
                Id = "1"
            }
        };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Image signature is invalid"
        });
    }

    [Test]
    public async Task ValidateAsync_When_FirstNameIsNotEmpty_LastNameIsNotEmpty_ImageIsNull_ShouldBeValid()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            FirstName = "test",
            LastName = "test",
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
    public async Task ValidateAsync_When_FirstNameIsNotEmpty_LastNameIsNotEmpty_ImageIsNotNull_SignatureIsValid_ShouldBeValid()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            FirstName = "test",
            LastName = "test",
            Image = new ImageRequestModel
            {
                Id = "1"
            }
        };
        var cancellationToken = new CancellationToken();
        _mockSignPackageService
            .Setup(x => x.Validate(request.Image))
            .Returns(true);

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }
}
#pragma warning restore CA1861 // Avoid constant arrays as arguments
using FluentAssertions;
using LetsTalk.Server.API.Models.Message;
using LetsTalk.Server.Core.Features.Profile.Commands.UpdateProfile;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions.Models;
using LetsTalk.Server.Persistence.Enums;
using LetsTalk.Server.SignPackage.Abstractions;
using Moq;

namespace LetsTalk.Server.UnitTests.Tests.Validators;

#pragma warning disable CA1861 // Avoid constant arrays as arguments

[TestFixture]
public class UpdateProfileCommandValidatorTests
{
    private const string SampleAccountId = "1";

    private UpdateProfileCommandValidator _validator;
    private Mock<ISignPackageService> _mockSignPackageService;
    private Mock<IAccountAgnosticService> _mockAccountAgnosticService;

    [SetUp]
    public void SetUp()
    {
        _mockSignPackageService = new Mock<ISignPackageService>();
        _mockAccountAgnosticService = new Mock<IAccountAgnosticService>();
        _validator = new(_mockSignPackageService.Object, _mockAccountAgnosticService.Object);
    }

    [Test]
    public async Task ValidateAsync_When_ModelIsEmpty_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            AccountId = SampleAccountId
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(x => x.GetByIdAsync(SampleAccountId, cancellationToken))
            .ReturnsAsync(new ProfileServiceModel
            {
                AccountTypeId = (int)AccountTypes.Facebook
            });

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
    public async Task ValidateAsync_When_FirstNameIsEmpty_LastNameIsEmpty_EmailIsNull_ImageIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            AccountId = SampleAccountId,
            FirstName = string.Empty,
            LastName = string.Empty
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(x => x.GetByIdAsync(SampleAccountId, cancellationToken))
            .ReturnsAsync(new ProfileServiceModel
            {
                AccountTypeId = (int)AccountTypes.Facebook
            });

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
    public async Task ValidateAsync_When_FirstNameIsNotEmpty_LastNameIsNotEmpty_EmailIsEmpty_RegisteredViaFB_ImageIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            AccountId = SampleAccountId,
            FirstName = "test",
            LastName = "test",
            Email = string.Empty
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(x => x.GetByIdAsync(SampleAccountId, cancellationToken))
            .ReturnsAsync(new ProfileServiceModel
            {
                AccountTypeId = (int)AccountTypes.Facebook
            });

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Email is invalid"
        });
    }

    [Test]
    public async Task ValidateAsync_When_FirstNameIsNotEmpty_LastNameIsNotEmpty_EmailIsEmpty_RegisteredByEmail_ImageIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            AccountId = SampleAccountId,
            FirstName = "test",
            LastName = "test",
            Email = string.Empty
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(x => x.GetByIdAsync(SampleAccountId, cancellationToken))
            .ReturnsAsync(new ProfileServiceModel
            {
                AccountTypeId = (int)AccountTypes.Email
            });

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Email is invalid",
            "Email cannot be empty for this account type"
        });
    }

    [Test]
    [TestCase("test")]
    [TestCase("test.com")]
    [TestCase("@com")]
    public async Task ValidateAsync_When_FirstNameIsNotEmpty_LastNameIsNotEmpty_EmailIsInvalid_ImageIsNull_ShouldContainValidationErrors
        (string email)
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            FirstName = "test",
            LastName = "test",
            Email = email
        };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Email is invalid"
        });
    }

    [Test]
    public async Task ValidateAsync_When_FirstNameIsNotEmpty_LastNameIsNotEmpty_EmailIsValid_ImageIsNotNull_SignatureIsInvalid_ShouldBeValid()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            FirstName = "test",
            LastName = "test",
            Email = "test@localhost.com",
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
    public async Task ValidateAsync_When_FirstNameIsNotEmpty_LastNameIsNotEmpty_EmailIsNull_ImageIsNull_ShouldBeValid()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            AccountId = SampleAccountId,
            FirstName = "test",
            LastName = "test"
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(x => x.GetByIdAsync(SampleAccountId, cancellationToken))
            .ReturnsAsync(new ProfileServiceModel
            {
                AccountTypeId = (int)AccountTypes.Facebook
            });

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEmpty();
    }

    [Test]
    public async Task ValidateAsync_When_FirstNameIsNotEmpty_LastNameIsNotEmpty_EmailIsValid_ImageIsNull_ShouldBeValid()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            FirstName = "test",
            LastName = "test",
            Email = "test@localhost.com"
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
    public async Task ValidateAsync_When_FirstNameIsNotEmpty_LastNameIsNotEmpty_EmailIsValid_ImageIsNotNull_SignatureIsValid_ShouldBeValid()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            FirstName = "test",
            LastName = "test",
            Email = "test@localhost.com",
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
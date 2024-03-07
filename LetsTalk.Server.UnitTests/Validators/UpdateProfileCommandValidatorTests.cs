using FluentAssertions;
using LetsTalk.Server.API.Models.Messages;
using LetsTalk.Server.Core.Features.Account.Commands.UpdateProfileCommand;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.SignPackage.Abstractions;
using Moq;

namespace LetsTalk.Server.UnitTests.Validators;

#pragma warning disable CA1861 // Avoid constant arrays as arguments

[TestFixture]
public class UpdateProfileCommandValidatorTests
{
    private UpdateProfileCommandValidator _validator;
    private Mock<IAccountAgnosticService> _mockAccountAgnosticService;
    private Mock<ISignPackageService> _mockSignPackageService;

    [SetUp]
    public void SetUp()
    {
        _mockAccountAgnosticService = new Mock<IAccountAgnosticService>();
        _mockSignPackageService = new Mock<ISignPackageService>();
        _validator = new(_mockAccountAgnosticService.Object, _mockSignPackageService.Object);
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
        validationResult.Errors.Should().HaveCount(5);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Account Id is required",
            "First Name is required",
            "First Name cannot be empty",
            "Last Name is required",
            "Last Name cannot be empty",
        });
    }

    [Test]
    public async Task ValidateAsync_When_AccountIdIsZero_AccountDoesNotExist_FirstNameIsNull_LastNameIsNull_EmailIsNull_ImageIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            AccountId = "0"
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(m => m.IsAccountIdValidAsync("0", cancellationToken))
            .Returns(Task.FromResult(false));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(5);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Account with Account Id = 0 does not exist",
            "First Name is required",
            "First Name cannot be empty",
            "Last Name is required",
            "Last Name cannot be empty",
        });
    }

    [Test]
    public async Task ValidateAsync_When_AccountIdIsZero_AccountExists_FirstNameIsNull_LastNameIsNull_EmailIsNull_ImageIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            AccountId = "0"
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(m => m.IsAccountIdValidAsync("0", cancellationToken))
            .Returns(Task.FromResult(true));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(4);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "First Name is required",
            "First Name cannot be empty",
            "Last Name is required",
            "Last Name cannot be empty",
        });
    }

    [Test]
    public async Task ValidateAsync_When_AccountIdIsZero_AccountExists_FirstNameIsEmpty_LastNameIsEmpty_EmailIsNull_ImageIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            AccountId = "0",
            FirstName = string.Empty,
            LastName = string.Empty
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(m => m.IsAccountIdValidAsync("0", cancellationToken))
            .Returns(Task.FromResult(true));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(2);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "First Name cannot be empty",
            "Last Name cannot be empty",
        });
    }

    [Test]
    public async Task ValidateAsync_When_AccountIdIsZero_AccountExists_FirstNameIsNotEmpty_LastNameIsNotEmpty_EmailIsEmpty_ImageIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            AccountId = "0",
            FirstName = "test",
            LastName = "test",
            Email = string.Empty
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(m => m.IsAccountIdValidAsync("0", cancellationToken))
            .Returns(Task.FromResult(true));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Email is invalid"
        });
    }

    [Test]
    [TestCase("test")]
    [TestCase("test.com")]
    [TestCase("@com")]
    public async Task ValidateAsync_When_AccountIdIsZero_AccountExists_FirstNameIsNotEmpty_LastNameIsNotEmpty_EmailIsInvalid_ImageIsNull_ShouldContainValidationErrors
        (string email)
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            AccountId = "0",
            FirstName = "test",
            LastName = "test",
            Email = email
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(m => m.IsAccountIdValidAsync("0", cancellationToken))
            .Returns(Task.FromResult(true));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Email is invalid"
        });
    }

    [Test]
    public async Task ValidateAsync_When_AccountIdIsZero_AccountExists_FirstNameIsNotEmpty_LastNameIsNotEmpty_EmailIsValid_ImageIsNotNull_SignatureIsInvalid_ShouldBeValid()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            AccountId = "0",
            FirstName = "test",
            LastName = "test",
            Email = "test@localhost.com",
            Image = new ImageRequestModel
            {
                Id = "1"
            }
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(m => m.IsAccountIdValidAsync("0", cancellationToken))
            .Returns(Task.FromResult(true));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Image signature is invalid"
        });
    }

    [Test]
    public async Task ValidateAsync_When_AccountIdIsZero_AccountExists_FirstNameIsNotEmpty_LastNameIsNotEmpty_EmailIsNull_ImageIsNull_ShouldBeValid()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            AccountId = "0",
            FirstName = "test",
            LastName = "test"
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(m => m.IsAccountIdValidAsync("0", cancellationToken))
            .Returns(Task.FromResult(true));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEmpty();
    }

    [Test]
    public async Task ValidateAsync_When_AccountIdIsZero_AccountExists_FirstNameIsNotEmpty_LastNameIsNotEmpty_EmailIsValid_ImageIsNull_ShouldBeValid()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            AccountId = "0",
            FirstName = "test",
            LastName = "test",
            Email = "test@localhost.com"
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(m => m.IsAccountIdValidAsync("0", cancellationToken))
            .Returns(Task.FromResult(true));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }

    [Test]
    public async Task ValidateAsync_When_AccountIdIsZero_AccountExists_FirstNameIsNotEmpty_LastNameIsNotEmpty_EmailIsValid_ImageIsNotNull_SignatureIsValid_ShouldBeValid()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            AccountId = "0",
            FirstName = "test",
            LastName = "test",
            Email = "test@localhost.com",
            Image = new ImageRequestModel
            {
                Id = "1"
            }
        };
        var cancellationToken = new CancellationToken();
        _mockAccountAgnosticService
            .Setup(m => m.IsAccountIdValidAsync("0", cancellationToken))
            .Returns(Task.FromResult(true));
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
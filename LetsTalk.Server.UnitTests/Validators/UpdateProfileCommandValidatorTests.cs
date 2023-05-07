using FluentAssertions;
using LetsTalk.Server.Core.Features.Profile.Commands.UpdateProfileCommand;
using LetsTalk.Server.Persistence.Abstractions;
using Moq;

namespace LetsTalk.Server.UnitTests.Validators;

[TestFixture]
public class UpdateProfileCommandValidatorTests
{
    private UpdateProfileCommandValidator _validator;
    private Mock<IAccountRepository> _mockAccountRepository;

    [SetUp]
    public void SetUp()
    {
        _mockAccountRepository = new Mock<IAccountRepository>();
        _validator = new(_mockAccountRepository.Object);
    }

    [Test]
    public async Task UpdateProfileCommandValidator_EmptyModel()
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
    public async Task UpdateProfileCommandValidator_AccountIdIsZero_AccountDoesNotExist_FirstNameIsNull_LastNameIsNull_EmailIsNull_PhotoUrlIsNull()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            AccountId = 0
        };
        var cancellationToken = new CancellationToken();
        _mockAccountRepository
            .Setup(m => m.IsAccountIdValidAsync(0))
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
    public async Task UpdateProfileCommandValidator_AccountIdIsZero_AccountExists_FirstNameIsNull_LastNameIsNull_EmailIsNull_PhotoUrlIsNull()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            AccountId = 0
        };
        var cancellationToken = new CancellationToken();
        _mockAccountRepository
            .Setup(m => m.IsAccountIdValidAsync(0))
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
    public async Task UpdateProfileCommandValidator_AccountIdIsZero_AccountExists_FirstNameIsEmpty_LastNameIsEmpty_EmailIsNull_PhotoUrlIsNull()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            AccountId = 0,
            FirstName = string.Empty,
            LastName = string.Empty
        };
        var cancellationToken = new CancellationToken();
        _mockAccountRepository
            .Setup(m => m.IsAccountIdValidAsync(0))
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
    public async Task UpdateProfileCommandValidator_AccountIdIsZero_AccountExists_FirstNameIsNotEmpty_LastNameIsNotEmpty_EmailIsNull_PhotoUrlIsNull()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            AccountId = 0,
            FirstName = "test",
            LastName = "test"
        };
        var cancellationToken = new CancellationToken();
        _mockAccountRepository
            .Setup(m => m.IsAccountIdValidAsync(0))
            .Returns(Task.FromResult(true));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEmpty();
    }

    [Test]
    public async Task UpdateProfileCommandValidator_AccountIdIsZero_AccountExists_FirstNameIsNotEmpty_LastNameIsNotEmpty_EmailIsEmpty_PhotoUrlIsNull()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            AccountId = 0,
            FirstName = "test",
            LastName = "test",
            Email = string.Empty
        };
        var cancellationToken = new CancellationToken();
        _mockAccountRepository
            .Setup(m => m.IsAccountIdValidAsync(0))
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
    public async Task UpdateProfileCommandValidator_AccountIdIsZero_AccountExists_FirstNameIsNotEmpty_LastNameIsNotEmpty_EmailIsInvalid_PhotoUrlIsNull(string email)
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            AccountId = 0,
            FirstName = "test",
            LastName = "test",
            Email = email
        };
        var cancellationToken = new CancellationToken();
        _mockAccountRepository
            .Setup(m => m.IsAccountIdValidAsync(0))
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
    public async Task UpdateProfileCommandValidator_AccountIdIsZero_AccountExists_FirstNameIsNotEmpty_LastNameIsNotEmpty_EmailIsValid_PhotoUrlIsNull()
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            AccountId = 0,
            FirstName = "test",
            LastName = "test",
            Email = "test@localhost.com"
        };
        var cancellationToken = new CancellationToken();
        _mockAccountRepository
            .Setup(m => m.IsAccountIdValidAsync(0))
            .Returns(Task.FromResult(true));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }

    [Test]
    [TestCase("test")]
    public async Task UpdateProfileCommandValidator_AccountIdIsZero_AccountExists_FirstNameIsNotEmpty_LastNameIsNotEmpty_EmailIsValid_PhotoUrlIsInvalid(string photoUrl)
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            AccountId = 0,
            FirstName = "test",
            LastName = "test",
            Email = "test@localhost.com",
            PhotoUrl = photoUrl
        };
        var cancellationToken = new CancellationToken();
        _mockAccountRepository
            .Setup(m => m.IsAccountIdValidAsync(0))
            .Returns(Task.FromResult(true));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().HaveCount(1);
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Photo Url is invalid base64 string or url"
        });
    }

    [Test]
    [TestCase("data:image/jpeg;base64,/9j/2wBDAAMCAg")]
    [TestCase("data:image/png;base64,/9j/2wBDAAMCAg")]
    [TestCase("data:image/gif;base64,/9j/2wBDAAMCAg")]
    [TestCase("https://localhost/")]
    public async Task UpdateProfileCommandValidator_AccountIdIsZero_AccountExists_FirstNameIsNotEmpty_LastNameIsNotEmpty_EmailIsValid_PhotoUrlIsValid(string photoUrl)
    {
        // Arrange
        var request = new UpdateProfileCommand
        {
            AccountId = 0,
            FirstName = "test",
            LastName = "test",
            Email = "test@localhost.com",
            PhotoUrl = photoUrl
        };
        var cancellationToken = new CancellationToken();
        _mockAccountRepository
            .Setup(m => m.IsAccountIdValidAsync(0))
            .Returns(Task.FromResult(true));

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }
}

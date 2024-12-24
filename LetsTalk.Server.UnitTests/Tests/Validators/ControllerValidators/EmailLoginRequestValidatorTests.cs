using FluentAssertions;
using LetsTalk.Server.API.Models.Login;
using LetsTalk.Server.API.Validation;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Utility.Common;

namespace LetsTalk.Server.UnitTests.Tests.Validators.ControllerValidators;

[TestFixture]
public class EmailLoginRequestValidatorTests
{
    private const string ValidEmailSample = "email@example.com";

    private EmailLoginRequestValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new(new SecuritySettings
        {
            AntiSpamTokenLifeTimeInSeconds = 60
        });
    }

    [Test]
    public async Task ValidateAsync_When_EmailIsNull_ShouldContainValidationErrors()
    {
        // Arrange
        var request = new EmailLoginRequest();
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Email is required",
            "Email cannot be empty",
            "Code should contain 4 digits",
            "Anti Spam Token cannot be empty",
            "Anti-spam check failed"
        });
    }

    [Test]
    [TestCase("")]
    [TestCase(" ")]
    public async Task ValidateAsync_When_EmailIsEmpty_ShouldContainValidationErrors(string email)
    {
        // Arrange
        var request = new EmailLoginRequest
        {
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
            "Email cannot be empty",
            "Email must be a valid email",
            "Code should contain 4 digits",
            "Anti Spam Token cannot be empty",
            "Anti-spam check failed"
        });
    }

    [Test]
    [TestCase("plainaddress")]
    [TestCase("#@%^%#$@#$@#.com")]
    [TestCase("@example.com")]
    [TestCase("email.example.com")]
    [TestCase("email@example@example.com")]
    public async Task ValidateAsync_When_EmailIsInvalid_ShouldContainValidationErrors(string email)
    {
        // Arrange
        var request = new EmailLoginRequest
        {
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
            "Email must be a valid email",
            "Code should contain 4 digits",
            "Anti Spam Token cannot be empty",
            "Anti-spam check failed"
        });
    }

    [Test]
    [TestCase(1)]
    [TestCase(11)]
    [TestCase(111)]
    [TestCase(11111)]
    [TestCase(-1111)]
    public async Task ValidateAsync_When_EmailIsValid_CodeIsInvalid_ShouldContainValidationErrors(int code)
    {
        // Arrange
        var request = new EmailLoginRequest
        {
            Email = ValidEmailSample,
            Code = code
        };
        var cancellationToken = new CancellationToken();

        // Act
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Select(error => error.ErrorMessage).Should().BeEquivalentTo(new string[]
        {
            "Code should contain 4 digits",
            "Anti Spam Token cannot be empty",
            "Anti-spam check failed"
        });
    }

    [Test]
    [TestCase(1234, 0)]
    [TestCase(9876, 1)]
    [TestCase(5783, -1)]
    [TestCase(8173, -59)]
    [TestCase(3520, 59)]
    public async Task ValidateAsync_When_EmailIsValid_CodeISValid_AntiSpamTokenIsValid_ShouldBeValid(int code, long delta)
    {
        // Arrange
        var request = new EmailLoginRequest
        {
            Email = ValidEmailSample,
            Code = code,
            AntiSpamToken = DateHelper.GetUnixTimestamp() + delta
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

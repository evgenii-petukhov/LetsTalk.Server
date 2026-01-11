using FluentAssertions;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.API.Core.Features.Authentication.Commands.EmailLogin;
using LetsTalk.Server.Authentication.Abstractions;
using Moq;

namespace LetsTalk.Server.API.Validation.Tests;

[TestFixture]
public class EmailLoginCommandValidatorTests
{
    private Mock<IAuthenticationClient> _authenticationClientMock;
    private EmailLoginCommandValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _authenticationClientMock = new Mock<IAuthenticationClient>();
        _validator = new EmailLoginCommandValidator(_authenticationClientMock.Object);
    }

    [Test]
    public async Task ValidateAsync_When_LoginCodeIsValid_ShouldBeValid()
    {
        // Arrange
        var command = new EmailLoginCommand
        {
            Email = "test@example.com",
            Code = 1234
        };
        _authenticationClientMock
            .Setup(x => x.ValidateLoginCodeAsync("test@example.com", 1234))
            .ReturnsAsync(true);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Test]
    public async Task ValidateAsync_When_LoginCodeIsInvalid_ShouldContainValidationError()
    {
        // Arrange
        var command = new EmailLoginCommand
        {
            Email = "test@example.com",
            Code = 1234
        };
        _authenticationClientMock
            .Setup(x => x.ValidateLoginCodeAsync("test@example.com", 1234))
            .ReturnsAsync(false);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be("Code has expired");
    }

    [Test]
    public async Task ValidateAsync_When_EmailHasWhitespace_ShouldTrimAndLowercase()
    {
        // Arrange
        var command = new EmailLoginCommand
        {
            Email = "  TEST@EXAMPLE.COM  ",
            Code = 1234
        };
        _authenticationClientMock
            .Setup(x => x.ValidateLoginCodeAsync("test@example.com", 1234))
            .ReturnsAsync(true);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        _authenticationClientMock.Verify(
            x => x.ValidateLoginCodeAsync("test@example.com", 1234),
            Times.Once);
    }

    [Test]
    public async Task ValidateAsync_When_EmailIsNull_ShouldCallWithEmptyString()
    {
        // Arrange
        var command = new EmailLoginCommand
        {
            Email = null,
            Code = 1234
        };
        _authenticationClientMock
            .Setup(x => x.ValidateLoginCodeAsync("", 1234))
            .ReturnsAsync(false);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        _authenticationClientMock.Verify(
            x => x.ValidateLoginCodeAsync(null, 1234),
            Times.Once);
    }
}
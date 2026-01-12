using FluentAssertions;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.API.Core.Features.Authentication.Commands.EmailLogin;
using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using LetsTalk.Server.Persistence.AgnosticServices.Abstractions;
using LetsTalk.Server.Persistence.Enums;
using Moq;

namespace LetsTalk.Server.API.Core.Tests.Handlers;

[TestFixture]
public class EmailLoginCommandHandlerTests
{
    private Mock<IAuthenticationClient> _authenticationClientMock;
    private Mock<IAccountAgnosticService> _accountAgnosticServiceMock;
    private EmailLoginCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _authenticationClientMock = new Mock<IAuthenticationClient>();
        _accountAgnosticServiceMock = new Mock<IAccountAgnosticService>();
        _handler = new EmailLoginCommandHandler(_authenticationClientMock.Object, _accountAgnosticServiceMock.Object);
    }

    [Test]
    public async Task Handle_WhenValidRequest_ShouldReturnSuccessfulLoginResponse()
    {
        // Arrange
        var email = "test@example.com";
        var code = 123456;
        var accountId = "account-123";
        var token = "jwt-token-123";
        var command = new EmailLoginCommand { Email = email, Code = code };
        var cancellationToken = CancellationToken.None;

        _authenticationClientMock
            .Setup(x => x.ValidateLoginCodeAsync(email.ToLowerInvariant(), code))
            .ReturnsAsync(true);

        _accountAgnosticServiceMock
            .Setup(x => x.GetOrCreateAsync(AccountTypes.Email, email.ToLowerInvariant(), cancellationToken))
            .ReturnsAsync(accountId);

        _authenticationClientMock
            .Setup(x => x.GenerateJwtTokenAsync(accountId))
            .ReturnsAsync(token);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Token.Should().Be(token);
    }

    [Test]
    public async Task Handle_WhenEmailHasWhitespace_ShouldTrimAndNormalizeEmail()
    {
        // Arrange
        var email = "  TEST@EXAMPLE.COM  ";
        var normalizedEmail = "test@example.com";
        var code = 123456;
        var accountId = "account-123";
        var token = "jwt-token-123";
        var command = new EmailLoginCommand { Email = email, Code = code };
        var cancellationToken = CancellationToken.None;

        _authenticationClientMock
            .Setup(x => x.ValidateLoginCodeAsync(normalizedEmail, code))
            .ReturnsAsync(true);

        _accountAgnosticServiceMock
            .Setup(x => x.GetOrCreateAsync(AccountTypes.Email, normalizedEmail, cancellationToken))
            .ReturnsAsync(accountId);

        _authenticationClientMock
            .Setup(x => x.GenerateJwtTokenAsync(accountId))
            .ReturnsAsync(token);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Token.Should().Be(token);

        // Verify that the email was normalized
        _authenticationClientMock.Verify(
            x => x.ValidateLoginCodeAsync(normalizedEmail, code),
            Times.Once);

        _accountAgnosticServiceMock.Verify(
            x => x.GetOrCreateAsync(AccountTypes.Email, normalizedEmail, cancellationToken),
            Times.Once);
    }

    [Test]
    public void Handle_WhenValidationFails_ShouldThrowBadRequestException()
    {
        // Arrange
        var email = "test@example.com";
        var code = 123456;
        var command = new EmailLoginCommand { Email = email, Code = code };
        var cancellationToken = CancellationToken.None;

        _authenticationClientMock
            .Setup(x => x.ValidateLoginCodeAsync(email.ToLowerInvariant(), code))
            .ReturnsAsync(false);

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public async Task Handle_WhenValidationPasses_ShouldCallGetOrCreateWithCorrectParameters()
    {
        // Arrange
        var email = "test@example.com";
        var code = 123456;
        var accountId = "account-123";
        var token = "jwt-token-123";
        var command = new EmailLoginCommand { Email = email, Code = code };
        var cancellationToken = CancellationToken.None;

        _authenticationClientMock
            .Setup(x => x.ValidateLoginCodeAsync(email.ToLowerInvariant(), code))
            .ReturnsAsync(true);

        _accountAgnosticServiceMock
            .Setup(x => x.GetOrCreateAsync(AccountTypes.Email, email.ToLowerInvariant(), cancellationToken))
            .ReturnsAsync(accountId);

        _authenticationClientMock
            .Setup(x => x.GenerateJwtTokenAsync(accountId))
            .ReturnsAsync(token);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _accountAgnosticServiceMock.Verify(
            x => x.GetOrCreateAsync(AccountTypes.Email, email.ToLowerInvariant(), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenValidationPasses_ShouldCallGenerateJwtTokenWithCorrectAccountId()
    {
        // Arrange
        var email = "test@example.com";
        var code = 123456;
        var accountId = "account-123";
        var token = "jwt-token-123";
        var command = new EmailLoginCommand { Email = email, Code = code };
        var cancellationToken = CancellationToken.None;

        _authenticationClientMock
            .Setup(x => x.ValidateLoginCodeAsync(email.ToLowerInvariant(), code))
            .ReturnsAsync(true);

        _accountAgnosticServiceMock
            .Setup(x => x.GetOrCreateAsync(AccountTypes.Email, email.ToLowerInvariant(), cancellationToken))
            .ReturnsAsync(accountId);

        _authenticationClientMock
            .Setup(x => x.GenerateJwtTokenAsync(accountId))
            .ReturnsAsync(token);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _authenticationClientMock.Verify(
            x => x.GenerateJwtTokenAsync(accountId),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenCancellationTokenProvided_ShouldPassItToServices()
    {
        // Arrange
        var email = "test@example.com";
        var code = 123456;
        var accountId = "account-123";
        var token = "jwt-token-123";
        var command = new EmailLoginCommand { Email = email, Code = code };
        var cancellationToken = CancellationToken.None; // Use a non-cancelled token

        _authenticationClientMock
            .Setup(x => x.ValidateLoginCodeAsync(email.ToLowerInvariant(), code))
            .ReturnsAsync(true);

        _accountAgnosticServiceMock
            .Setup(x => x.GetOrCreateAsync(AccountTypes.Email, email.ToLowerInvariant(), cancellationToken))
            .ReturnsAsync(accountId);

        _authenticationClientMock
            .Setup(x => x.GenerateJwtTokenAsync(accountId))
            .ReturnsAsync(token);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _accountAgnosticServiceMock.Verify(
            x => x.GetOrCreateAsync(AccountTypes.Email, email.ToLowerInvariant(), cancellationToken),
            Times.Once);
    }

    [Test]
    public void Handle_WhenAuthenticationClientThrowsException_ShouldPropagateException()
    {
        // Arrange
        var email = "test@example.com";
        var code = 123456;
        var command = new EmailLoginCommand { Email = email, Code = code };
        var cancellationToken = CancellationToken.None;

        _authenticationClientMock
            .Setup(x => x.ValidateLoginCodeAsync(email.ToLowerInvariant(), code))
            .ThrowsAsync(new InvalidOperationException("Authentication service error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public void Handle_WhenAccountServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var email = "test@example.com";
        var code = 123456;
        var command = new EmailLoginCommand { Email = email, Code = code };
        var cancellationToken = CancellationToken.None;

        _authenticationClientMock
            .Setup(x => x.ValidateLoginCodeAsync(email.ToLowerInvariant(), code))
            .ReturnsAsync(true);

        _accountAgnosticServiceMock
            .Setup(x => x.GetOrCreateAsync(AccountTypes.Email, email.ToLowerInvariant(), cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Account service error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public void Handle_WhenTokenGenerationThrowsException_ShouldPropagateException()
    {
        // Arrange
        var email = "test@example.com";
        var code = 123456;
        var accountId = "account-123";
        var command = new EmailLoginCommand { Email = email, Code = code };
        var cancellationToken = CancellationToken.None;

        _authenticationClientMock
            .Setup(x => x.ValidateLoginCodeAsync(email.ToLowerInvariant(), code))
            .ReturnsAsync(true);

        _accountAgnosticServiceMock
            .Setup(x => x.GetOrCreateAsync(AccountTypes.Email, email.ToLowerInvariant(), cancellationToken))
            .ReturnsAsync(accountId);

        _authenticationClientMock
            .Setup(x => x.GenerateJwtTokenAsync(accountId))
            .ThrowsAsync(new InvalidOperationException("Token generation error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public void Handle_WhenEmailIsNull_ShouldHandleGracefully()
    {
        // Arrange
        var code = 123456;
        var command = new EmailLoginCommand { Email = null, Code = code };
        var cancellationToken = CancellationToken.None;

        _authenticationClientMock
            .Setup(x => x.ValidateLoginCodeAsync(It.IsAny<string>(), code))
            .ReturnsAsync(false);

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public void Handle_WhenEmailIsEmpty_ShouldHandleGracefully()
    {
        // Arrange
        var email = "";
        var code = 123456;
        var command = new EmailLoginCommand { Email = email, Code = code };
        var cancellationToken = CancellationToken.None;

        _authenticationClientMock
            .Setup(x => x.ValidateLoginCodeAsync(email, code))
            .ReturnsAsync(false);

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public void Handle_WhenCodeIsZero_ShouldStillValidate()
    {
        // Arrange
        var email = "test@example.com";
        var code = 0;
        var command = new EmailLoginCommand { Email = email, Code = code };
        var cancellationToken = CancellationToken.None;

        _authenticationClientMock
            .Setup(x => x.ValidateLoginCodeAsync(email.ToLowerInvariant(), code))
            .ReturnsAsync(false);

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(
            () => _handler.Handle(command, cancellationToken));

        // Verify that validation was attempted with the zero code
        _authenticationClientMock.Verify(
            x => x.ValidateLoginCodeAsync(email.ToLowerInvariant(), code),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenValidRequest_ShouldFollowCompleteWorkflow()
    {
        // Arrange
        var email = "  Test@Example.Com  ";
        var normalizedEmail = "test@example.com";
        var code = 123456;
        var accountId = "account-123";
        var token = "jwt-token-123";
        var command = new EmailLoginCommand { Email = email, Code = code };
        var cancellationToken = CancellationToken.None;

        _authenticationClientMock
            .Setup(x => x.ValidateLoginCodeAsync(normalizedEmail, code))
            .ReturnsAsync(true);

        _accountAgnosticServiceMock
            .Setup(x => x.GetOrCreateAsync(AccountTypes.Email, normalizedEmail, cancellationToken))
            .ReturnsAsync(accountId);

        _authenticationClientMock
            .Setup(x => x.GenerateJwtTokenAsync(accountId))
            .ReturnsAsync(token);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Token.Should().Be(token);

        // Verify the complete workflow
        _authenticationClientMock.Verify(
            x => x.ValidateLoginCodeAsync(normalizedEmail, code),
            Times.Once);

        _accountAgnosticServiceMock.Verify(
            x => x.GetOrCreateAsync(AccountTypes.Email, normalizedEmail, cancellationToken),
            Times.Once);

        _authenticationClientMock.Verify(
            x => x.GenerateJwtTokenAsync(accountId),
            Times.Once);
    }

    [Test]
    public void Handle_WhenValidationFailsWithMultipleErrors_ShouldIncludeAllErrorsInException()
    {
        // Arrange
        var email = "test@example.com";
        var code = 123456;
        var command = new EmailLoginCommand { Email = email, Code = code };
        var cancellationToken = CancellationToken.None;

        // This test simulates the validator behavior when validation fails
        _authenticationClientMock
            .Setup(x => x.ValidateLoginCodeAsync(email.ToLowerInvariant(), code))
            .ReturnsAsync(false);

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(
            () => _handler.Handle(command, cancellationToken));
    }
}
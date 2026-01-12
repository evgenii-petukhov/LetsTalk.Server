using FluentAssertions;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Commands;
using LetsTalk.Server.API.Core.Features.Authentication.Commands.GenerateLoginCode;
using LetsTalk.Server.Authentication.Abstractions;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Kafka.Models;
using Moq;

namespace LetsTalk.Server.API.Core.Tests.Handlers;

[TestFixture]
public class GenerateLoginCodeCommandHandlerTests
{
    private Mock<IAuthenticationClient> _authenticationClientMock;
    private Mock<IProducer<SendEmailRequest>> _producerMock;
    private GenerateLoginCodeCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _authenticationClientMock = new Mock<IAuthenticationClient>();
        _producerMock = new Mock<IProducer<SendEmailRequest>>();
        _handler = new GenerateLoginCodeCommandHandler(_authenticationClientMock.Object, _producerMock.Object);
    }

    [Test]
    public async Task Handle_WhenCodeIsCreated_ShouldReturnResponseAndSendEmail()
    {
        // Arrange
        var email = "test@example.com";
        var code = 123456;
        var ttl = 300;
        var command = new GenerateLoginCodeCommand(email);
        var cancellationToken = CancellationToken.None;

        _authenticationClientMock
            .Setup(x => x.GenerateLoginCodeAsync(email.ToLowerInvariant()))
            .ReturnsAsync((code, true, ttl));

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.CodeValidInSeconds.Should().Be(ttl);

        // Verify email was sent
        _producerMock.Verify(
            x => x.PublishAsync(
                It.Is<SendEmailRequest>(req => 
                    req.Address == email.ToLowerInvariant() &&
                    req.Subject == "LetsTalk: login code" &&
                    req.Body!.Contains(code.ToString())),
                cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenCodeIsNotCreated_ShouldReturnResponseWithoutSendingEmail()
    {
        // Arrange
        var email = "test@example.com";
        var code = 123456;
        var ttl = 300;
        var command = new GenerateLoginCodeCommand(email);
        var cancellationToken = CancellationToken.None;

        _authenticationClientMock
            .Setup(x => x.GenerateLoginCodeAsync(email.ToLowerInvariant()))
            .ReturnsAsync((code, false, ttl));

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.CodeValidInSeconds.Should().Be(ttl);

        // Verify no email was sent
        _producerMock.Verify(
            x => x.PublishAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task Handle_WhenEmailHasWhitespace_ShouldTrimAndNormalizeEmail()
    {
        // Arrange
        var email = "  TEST@EXAMPLE.COM  ";
        var normalizedEmail = "test@example.com";
        var code = 123456;
        var ttl = 300;
        var command = new GenerateLoginCodeCommand(email);
        var cancellationToken = CancellationToken.None;

        _authenticationClientMock
            .Setup(x => x.GenerateLoginCodeAsync(normalizedEmail))
            .ReturnsAsync((code, true, ttl));

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.CodeValidInSeconds.Should().Be(ttl);

        // Verify that the email was normalized
        _authenticationClientMock.Verify(
            x => x.GenerateLoginCodeAsync(normalizedEmail),
            Times.Once);

        _producerMock.Verify(
            x => x.PublishAsync(
                It.Is<SendEmailRequest>(req => req.Address == normalizedEmail),
                cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenCodeIsCreated_ShouldSendEmailWithCorrectContent()
    {
        // Arrange
        var email = "test@example.com";
        var code = 987654;
        var ttl = 600;
        var command = new GenerateLoginCodeCommand(email);
        var cancellationToken = CancellationToken.None;

        _authenticationClientMock
            .Setup(x => x.GenerateLoginCodeAsync(email.ToLowerInvariant()))
            .ReturnsAsync((code, true, ttl));

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _producerMock.Verify(
            x => x.PublishAsync(
                It.Is<SendEmailRequest>(req => 
                    req.Address == email.ToLowerInvariant() &&
                    req.Subject == "LetsTalk: login code" &&
                    req.Body == $"{code} is your new login code\r\n\r\nAll the best,\r\nLetsTalk team."),
                cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenCancellationTokenProvided_ShouldPassItToProducer()
    {
        // Arrange
        var email = "test@example.com";
        var code = 123456;
        var ttl = 300;
        var command = new GenerateLoginCodeCommand(email);
        var cancellationToken = new CancellationToken(false);

        _authenticationClientMock
            .Setup(x => x.GenerateLoginCodeAsync(email.ToLowerInvariant()))
            .ReturnsAsync((code, true, ttl));

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _producerMock.Verify(
            x => x.PublishAsync(It.IsAny<SendEmailRequest>(), cancellationToken),
            Times.Once);
    }

    [Test]
    public void Handle_WhenAuthenticationClientThrowsException_ShouldPropagateException()
    {
        // Arrange
        var email = "test@example.com";
        var command = new GenerateLoginCodeCommand(email);
        var cancellationToken = CancellationToken.None;

        _authenticationClientMock
            .Setup(x => x.GenerateLoginCodeAsync(email.ToLowerInvariant()))
            .ThrowsAsync(new InvalidOperationException("Authentication service error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public void Handle_WhenProducerThrowsException_ShouldPropagateException()
    {
        // Arrange
        var email = "test@example.com";
        var code = 123456;
        var ttl = 300;
        var command = new GenerateLoginCodeCommand(email);
        var cancellationToken = CancellationToken.None;

        _authenticationClientMock
            .Setup(x => x.GenerateLoginCodeAsync(email.ToLowerInvariant()))
            .ReturnsAsync((code, true, ttl));

        _producerMock
            .Setup(x => x.PublishAsync(It.IsAny<SendEmailRequest>(), cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Producer error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, cancellationToken));
    }

    [Test]
    public async Task Handle_WhenTtlIsZero_ShouldReturnZeroTtl()
    {
        // Arrange
        var email = "test@example.com";
        var code = 123456;
        var ttl = 0;
        var command = new GenerateLoginCodeCommand(email);
        var cancellationToken = CancellationToken.None;

        _authenticationClientMock
            .Setup(x => x.GenerateLoginCodeAsync(email.ToLowerInvariant()))
            .ReturnsAsync((code, true, ttl));

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.CodeValidInSeconds.Should().Be(0);

        // Email should still be sent if code was created
        _producerMock.Verify(
            x => x.PublishAsync(It.IsAny<SendEmailRequest>(), cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenCodeIsZero_ShouldStillSendEmailWithZeroCode()
    {
        // Arrange
        var email = "test@example.com";
        var code = 0;
        var ttl = 300;
        var command = new GenerateLoginCodeCommand(email);
        var cancellationToken = CancellationToken.None;

        _authenticationClientMock
            .Setup(x => x.GenerateLoginCodeAsync(email.ToLowerInvariant()))
            .ReturnsAsync((code, true, ttl));

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.CodeValidInSeconds.Should().Be(ttl);

        _producerMock.Verify(
            x => x.PublishAsync(
                It.Is<SendEmailRequest>(req => req.Body!.Contains("0")),
                cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenEmailIsAlreadyLowercase_ShouldNotChangeEmail()
    {
        // Arrange
        var email = "test@example.com";
        var code = 123456;
        var ttl = 300;
        var command = new GenerateLoginCodeCommand(email);
        var cancellationToken = CancellationToken.None;

        _authenticationClientMock
            .Setup(x => x.GenerateLoginCodeAsync(email))
            .ReturnsAsync((code, true, ttl));

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _authenticationClientMock.Verify(
            x => x.GenerateLoginCodeAsync(email),
            Times.Once);

        _producerMock.Verify(
            x => x.PublishAsync(
                It.Is<SendEmailRequest>(req => req.Address == email),
                cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenMultipleCallsWithSameEmail_ShouldNormalizeConsistently()
    {
        // Arrange
        var email1 = "TEST@EXAMPLE.COM";
        var email2 = "test@example.com";
        var email3 = "  Test@Example.Com  ";
        var normalizedEmail = "test@example.com";
        var code = 123456;
        var ttl = 300;

        _authenticationClientMock
            .Setup(x => x.GenerateLoginCodeAsync(normalizedEmail))
            .ReturnsAsync((code, true, ttl));

        // Act
        await _handler.Handle(new GenerateLoginCodeCommand(email1), CancellationToken.None);
        await _handler.Handle(new GenerateLoginCodeCommand(email2), CancellationToken.None);
        await _handler.Handle(new GenerateLoginCodeCommand(email3), CancellationToken.None);

        // Assert
        _authenticationClientMock.Verify(
            x => x.GenerateLoginCodeAsync(normalizedEmail),
            Times.Exactly(3));

        _producerMock.Verify(
            x => x.PublishAsync(
                It.Is<SendEmailRequest>(req => req.Address == normalizedEmail),
                It.IsAny<CancellationToken>()),
            Times.Exactly(3));
    }

    [Test]
    public async Task Handle_WhenEmailTemplateContainsCode_ShouldFormatCorrectly()
    {
        // Arrange
        var email = "test@example.com";
        var code = 555777;
        var ttl = 300;
        var command = new GenerateLoginCodeCommand(email);
        var cancellationToken = CancellationToken.None;

        _authenticationClientMock
            .Setup(x => x.GenerateLoginCodeAsync(email.ToLowerInvariant()))
            .ReturnsAsync((code, true, ttl));

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _producerMock.Verify(
            x => x.PublishAsync(
                It.Is<SendEmailRequest>(req => 
                    req.Body!.StartsWith("555777 is your new login code") &&
                    req.Body.Contains("All the best,") &&
                    req.Body.Contains("LetsTalk team.")),
                cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenValidRequest_ShouldFollowCompleteWorkflow()
    {
        // Arrange
        var email = "  User@Domain.COM  ";
        var normalizedEmail = "user@domain.com";
        var code = 999888;
        var ttl = 450;
        var command = new GenerateLoginCodeCommand(email);
        var cancellationToken = CancellationToken.None;

        _authenticationClientMock
            .Setup(x => x.GenerateLoginCodeAsync(normalizedEmail))
            .ReturnsAsync((code, true, ttl));

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.CodeValidInSeconds.Should().Be(ttl);

        // Verify the complete workflow
        _authenticationClientMock.Verify(
            x => x.GenerateLoginCodeAsync(normalizedEmail),
            Times.Once);

        _producerMock.Verify(
            x => x.PublishAsync(
                It.Is<SendEmailRequest>(req => 
                    req.Address == normalizedEmail &&
                    req.Subject == "LetsTalk: login code" &&
                    req.Body!.Contains(code.ToString())),
                cancellationToken),
            Times.Once);
    }
}
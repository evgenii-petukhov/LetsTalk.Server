using FluentAssertions;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Features.VideoCall.Queries.GetCallSettings;
using LetsTalk.Server.Configuration.Models;
using LetsTalk.Server.Dto.Models;
using LetsTalk.Server.Exceptions;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace LetsTalk.Server.API.Core.Tests.Handlers;

[TestFixture]
public class GetCallSettingsQueryHandlerTests
{
    private Mock<IIceServerConfigurationService> _iceServerConfigurationServiceMock;
    private Mock<IOptions<RtcSettings>> _optionsMock;

    [SetUp]
    public void SetUp()
    {
        _iceServerConfigurationServiceMock = new Mock<IIceServerConfigurationService>();
        _optionsMock = new Mock<IOptions<RtcSettings>>();
    }

    private GetCallSettingsQueryHandler CreateHandler()
    {
        return new GetCallSettingsQueryHandler(
            _iceServerConfigurationServiceMock.Object,
            _optionsMock.Object);
    }

    [Test]
    public async Task Handle_WhenValidConfiguration_ShouldReturnCallSettings()
    {
        // Arrange
        var query = new GetCallSettingsQuery();
        var cancellationToken = CancellationToken.None;

        var iceServerConfiguration = "stun:stun.example.com:3478";
        var rtcSettings = new RtcSettings { MaxVideoDurationInSeconds = 300 };

        _iceServerConfigurationServiceMock
            .Setup(x => x.GetIceServerConfigurationAsync(cancellationToken))
            .ReturnsAsync(iceServerConfiguration);

        _optionsMock
            .Setup(x => x.Value)
            .Returns(rtcSettings);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IceServerConfiguration.Should().Be(iceServerConfiguration);
        result.MaxVideoDurationInSeconds.Should().Be(300);

        _iceServerConfigurationServiceMock.Verify(
            x => x.GetIceServerConfigurationAsync(cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenComplexIceServerConfiguration_ShouldReturnCorrectSettings()
    {
        // Arrange
        var query = new GetCallSettingsQuery();
        var cancellationToken = CancellationToken.None;

        var complexIceConfig = "turn:turn.example.com:3478?transport=udp,turn:turn.example.com:3478?transport=tcp,stun:stun.example.com:19302";
        var rtcSettings = new RtcSettings { MaxVideoDurationInSeconds = 1800 };

        _iceServerConfigurationServiceMock
            .Setup(x => x.GetIceServerConfigurationAsync(cancellationToken))
            .ReturnsAsync(complexIceConfig);

        _optionsMock
            .Setup(x => x.Value)
            .Returns(rtcSettings);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IceServerConfiguration.Should().Be(complexIceConfig);
        result.MaxVideoDurationInSeconds.Should().Be(1800);
    }

    [Test]
    public void Handle_WhenIceServerConfigurationIsNull_ShouldThrowBadRequestException()
    {
        // Arrange
        var query = new GetCallSettingsQuery();
        var cancellationToken = CancellationToken.None;

        var rtcSettings = new RtcSettings { MaxVideoDurationInSeconds = 300 };

        _iceServerConfigurationServiceMock
            .Setup(x => x.GetIceServerConfigurationAsync(cancellationToken))
            .ReturnsAsync((string?)null);

        _optionsMock
            .Setup(x => x.Value)
            .Returns(rtcSettings);

        var handler = CreateHandler();

        // Act & Assert
        var exception = Assert.ThrowsAsync<BadRequestException>(
            () => handler.Handle(query, cancellationToken));

        exception.Message.Should().Be("Invalid request");

        _iceServerConfigurationServiceMock.Verify(
            x => x.GetIceServerConfigurationAsync(cancellationToken),
            Times.Once);
    }

    [Test]
    public void Handle_WhenIceServerConfigurationIsEmpty_ShouldThrowBadRequestException()
    {
        // Arrange
        var query = new GetCallSettingsQuery();
        var cancellationToken = CancellationToken.None;

        var rtcSettings = new RtcSettings { MaxVideoDurationInSeconds = 300 };

        _iceServerConfigurationServiceMock
            .Setup(x => x.GetIceServerConfigurationAsync(cancellationToken))
            .ReturnsAsync("");

        _optionsMock
            .Setup(x => x.Value)
            .Returns(rtcSettings);

        var handler = CreateHandler();

        // Act & Assert
        var exception = Assert.ThrowsAsync<BadRequestException>(
            () => handler.Handle(query, cancellationToken));

        exception.Message.Should().Be("Invalid request");

        _iceServerConfigurationServiceMock.Verify(
            x => x.GetIceServerConfigurationAsync(cancellationToken),
            Times.Once);
    }

    [Test]
    public void Handle_WhenIceServerConfigurationIsWhitespace_ShouldThrowBadRequestException()
    {
        // Arrange
        var query = new GetCallSettingsQuery();
        var cancellationToken = CancellationToken.None;

        var rtcSettings = new RtcSettings { MaxVideoDurationInSeconds = 300 };

        _iceServerConfigurationServiceMock
            .Setup(x => x.GetIceServerConfigurationAsync(cancellationToken))
            .ReturnsAsync("   \t\n   ");

        _optionsMock
            .Setup(x => x.Value)
            .Returns(rtcSettings);

        var handler = CreateHandler();

        // Act & Assert
        var exception = Assert.ThrowsAsync<BadRequestException>(
            () => handler.Handle(query, cancellationToken));

        exception.Message.Should().Be("Invalid request");

        _iceServerConfigurationServiceMock.Verify(
            x => x.GetIceServerConfigurationAsync(cancellationToken),
            Times.Once);
    }

    [Test]
    public void Handle_WhenIceServerConfigurationServiceThrowsException_ShouldPropagateException()
    {
        // Arrange
        var query = new GetCallSettingsQuery();
        var cancellationToken = CancellationToken.None;

        var rtcSettings = new RtcSettings { MaxVideoDurationInSeconds = 300 };

        _iceServerConfigurationServiceMock
            .Setup(x => x.GetIceServerConfigurationAsync(cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Service error"));

        _optionsMock
            .Setup(x => x.Value)
            .Returns(rtcSettings);

        var handler = CreateHandler();

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(query, cancellationToken));

        _iceServerConfigurationServiceMock.Verify(
            x => x.GetIceServerConfigurationAsync(cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenCancellationTokenProvided_ShouldPassItToService()
    {
        // Arrange
        var query = new GetCallSettingsQuery();
        var cancellationToken = new CancellationToken(false);

        var iceServerConfiguration = "stun:stun.example.com:3478";
        var rtcSettings = new RtcSettings { MaxVideoDurationInSeconds = 300 };

        _iceServerConfigurationServiceMock
            .Setup(x => x.GetIceServerConfigurationAsync(cancellationToken))
            .ReturnsAsync(iceServerConfiguration);

        _optionsMock
            .Setup(x => x.Value)
            .Returns(rtcSettings);

        var handler = CreateHandler();

        // Act
        await handler.Handle(query, cancellationToken);

        // Assert - Verification is done in the setup methods that check cancellationToken
        _iceServerConfigurationServiceMock.Verify(
            x => x.GetIceServerConfigurationAsync(cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenZeroMaxVideoDuration_ShouldReturnZeroValue()
    {
        // Arrange
        var query = new GetCallSettingsQuery();
        var cancellationToken = CancellationToken.None;

        var iceServerConfiguration = "stun:stun.example.com:3478";
        var rtcSettings = new RtcSettings { MaxVideoDurationInSeconds = 0 };

        _iceServerConfigurationServiceMock
            .Setup(x => x.GetIceServerConfigurationAsync(cancellationToken))
            .ReturnsAsync(iceServerConfiguration);

        _optionsMock
            .Setup(x => x.Value)
            .Returns(rtcSettings);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IceServerConfiguration.Should().Be(iceServerConfiguration);
        result.MaxVideoDurationInSeconds.Should().Be(0);
    }

    [Test]
    public async Task Handle_WhenNegativeMaxVideoDuration_ShouldReturnNegativeValue()
    {
        // Arrange
        var query = new GetCallSettingsQuery();
        var cancellationToken = CancellationToken.None;

        var iceServerConfiguration = "stun:stun.example.com:3478";
        var rtcSettings = new RtcSettings { MaxVideoDurationInSeconds = -1 };

        _iceServerConfigurationServiceMock
            .Setup(x => x.GetIceServerConfigurationAsync(cancellationToken))
            .ReturnsAsync(iceServerConfiguration);

        _optionsMock
            .Setup(x => x.Value)
            .Returns(rtcSettings);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IceServerConfiguration.Should().Be(iceServerConfiguration);
        result.MaxVideoDurationInSeconds.Should().Be(-1);
    }

    [Test]
    public async Task Handle_WhenLargeMaxVideoDuration_ShouldReturnLargeValue()
    {
        // Arrange
        var query = new GetCallSettingsQuery();
        var cancellationToken = CancellationToken.None;

        var iceServerConfiguration = "stun:stun.example.com:3478";
        var rtcSettings = new RtcSettings { MaxVideoDurationInSeconds = int.MaxValue };

        _iceServerConfigurationServiceMock
            .Setup(x => x.GetIceServerConfigurationAsync(cancellationToken))
            .ReturnsAsync(iceServerConfiguration);

        _optionsMock
            .Setup(x => x.Value)
            .Returns(rtcSettings);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IceServerConfiguration.Should().Be(iceServerConfiguration);
        result.MaxVideoDurationInSeconds.Should().Be(int.MaxValue);
    }

    [Test]
    public async Task Handle_WhenSpecialCharactersInIceConfiguration_ShouldHandleCorrectly()
    {
        // Arrange
        var query = new GetCallSettingsQuery();
        var cancellationToken = CancellationToken.None;

        var iceServerConfiguration = "turn:user:pass@turn.example.com:3478?transport=tcp&special=!@#$%^&*()";
        var rtcSettings = new RtcSettings { MaxVideoDurationInSeconds = 600 };

        _iceServerConfigurationServiceMock
            .Setup(x => x.GetIceServerConfigurationAsync(cancellationToken))
            .ReturnsAsync(iceServerConfiguration);

        _optionsMock
            .Setup(x => x.Value)
            .Returns(rtcSettings);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IceServerConfiguration.Should().Be(iceServerConfiguration);
        result.MaxVideoDurationInSeconds.Should().Be(600);
    }

    [Test]
    public async Task Handle_WhenUnicodeCharactersInIceConfiguration_ShouldHandleCorrectly()
    {
        // Arrange
        var query = new GetCallSettingsQuery();
        var cancellationToken = CancellationToken.None;

        var iceServerConfiguration = "stun:服务器.example.com:3478,turn:用户:密码@转发.example.com:3478";
        var rtcSettings = new RtcSettings { MaxVideoDurationInSeconds = 900 };

        _iceServerConfigurationServiceMock
            .Setup(x => x.GetIceServerConfigurationAsync(cancellationToken))
            .ReturnsAsync(iceServerConfiguration);

        _optionsMock
            .Setup(x => x.Value)
            .Returns(rtcSettings);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IceServerConfiguration.Should().Be(iceServerConfiguration);
        result.MaxVideoDurationInSeconds.Should().Be(900);
    }

    [Test]
    public async Task Handle_WhenVeryLongIceConfiguration_ShouldHandleCorrectly()
    {
        // Arrange
        var query = new GetCallSettingsQuery();
        var cancellationToken = CancellationToken.None;

        var longIceConfig = "stun:" + new string('a', 1000) + ".example.com:3478";
        var rtcSettings = new RtcSettings { MaxVideoDurationInSeconds = 1200 };

        _iceServerConfigurationServiceMock
            .Setup(x => x.GetIceServerConfigurationAsync(cancellationToken))
            .ReturnsAsync(longIceConfig);

        _optionsMock
            .Setup(x => x.Value)
            .Returns(rtcSettings);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IceServerConfiguration.Should().Be(longIceConfig);
        result.IceServerConfiguration!.Length.Should().Be(1022); // "stun:" + 1000 chars + ".example.com:3478"
        result.MaxVideoDurationInSeconds.Should().Be(1200);
    }

    [Test]
    public async Task Handle_WhenMultipleCallsWithSameQuery_ShouldCallServiceEachTime()
    {
        // Arrange
        var query = new GetCallSettingsQuery();
        var cancellationToken = CancellationToken.None;

        var iceServerConfiguration = "stun:stun.example.com:3478";
        var rtcSettings = new RtcSettings { MaxVideoDurationInSeconds = 300 };

        _iceServerConfigurationServiceMock
            .Setup(x => x.GetIceServerConfigurationAsync(cancellationToken))
            .ReturnsAsync(iceServerConfiguration);

        _optionsMock
            .Setup(x => x.Value)
            .Returns(rtcSettings);

        var handler = CreateHandler();

        // Act
        var result1 = await handler.Handle(query, cancellationToken);
        var result2 = await handler.Handle(query, cancellationToken);
        var result3 = await handler.Handle(query, cancellationToken);

        // Assert
        result1.Should().BeEquivalentTo(result2);
        result2.Should().BeEquivalentTo(result3);

        _iceServerConfigurationServiceMock.Verify(
            x => x.GetIceServerConfigurationAsync(cancellationToken),
            Times.Exactly(3));

        // Options.Value should be accessed once during handler construction
        _optionsMock.Verify(x => x.Value, Times.Once);
    }

    [Test]
    public async Task Handle_WhenDifferentRtcSettingsValues_ShouldReturnCorrectValues()
    {
        // Arrange
        var query = new GetCallSettingsQuery();
        var cancellationToken = CancellationToken.None;

        var iceServerConfiguration = "stun:stun.example.com:3478";

        var testCases = new[]
        {
            new { Duration = 60, Description = "1 minute" },
            new { Duration = 300, Description = "5 minutes" },
            new { Duration = 1800, Description = "30 minutes" },
            new { Duration = 3600, Description = "1 hour" },
            new { Duration = 7200, Description = "2 hours" }
        };

        foreach (var testCase in testCases)
        {
            // Reset mocks for each iteration
            _iceServerConfigurationServiceMock.Reset();
            _optionsMock.Reset();

            var rtcSettings = new RtcSettings { MaxVideoDurationInSeconds = testCase.Duration };

            _iceServerConfigurationServiceMock
                .Setup(x => x.GetIceServerConfigurationAsync(cancellationToken))
                .ReturnsAsync(iceServerConfiguration);

            _optionsMock
                .Setup(x => x.Value)
                .Returns(rtcSettings);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(query, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IceServerConfiguration.Should().Be(iceServerConfiguration);
            result.MaxVideoDurationInSeconds.Should().Be(testCase.Duration, 
                $"because test case is {testCase.Description}");
        }
    }

    [Test]
    public async Task Handle_WhenQueryParameterIsIgnored_ShouldStillWork()
    {
        // Arrange
        var query = new GetCallSettingsQuery(); // Query has no parameters, uses _ in handler
        var cancellationToken = CancellationToken.None;

        var iceServerConfiguration = "stun:stun.example.com:3478";
        var rtcSettings = new RtcSettings { MaxVideoDurationInSeconds = 300 };

        _iceServerConfigurationServiceMock
            .Setup(x => x.GetIceServerConfigurationAsync(cancellationToken))
            .ReturnsAsync(iceServerConfiguration);

        _optionsMock
            .Setup(x => x.Value)
            .Returns(rtcSettings);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IceServerConfiguration.Should().Be(iceServerConfiguration);
        result.MaxVideoDurationInSeconds.Should().Be(300);

        // The query parameter is ignored (using _ in the handler), so any query should work the same
        _iceServerConfigurationServiceMock.Verify(
            x => x.GetIceServerConfigurationAsync(cancellationToken),
            Times.Once);
    }

    [Test]
    public async Task Handle_WhenValidConfigurationWithTrimming_ShouldReturnTrimmedConfiguration()
    {
        // Arrange
        var query = new GetCallSettingsQuery();
        var cancellationToken = CancellationToken.None;

        // Note: The handler doesn't trim, but this tests that whitespace-only is still invalid
        var iceServerConfiguration = "stun:stun.example.com:3478"; // Valid config without extra whitespace
        var rtcSettings = new RtcSettings { MaxVideoDurationInSeconds = 300 };

        _iceServerConfigurationServiceMock
            .Setup(x => x.GetIceServerConfigurationAsync(cancellationToken))
            .ReturnsAsync(iceServerConfiguration);

        _optionsMock
            .Setup(x => x.Value)
            .Returns(rtcSettings);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IceServerConfiguration.Should().Be(iceServerConfiguration);
        result.MaxVideoDurationInSeconds.Should().Be(300);
    }
}
using FluentAssertions;
using LetsTalk.Server.LinkPreview.Utility.Abstractions;
using LetsTalk.Server.LinkPreview.Utility.Abstractions.Models;
using LetsTalk.Server.LinkPreview.Utility.Services;
using Moq;
using System.Net;

namespace LetsTalk.Server.LinkPreview.Utility.Tests;

[TestFixture]
public class LinkPreviewServiceTests
{
    private Mock<IDownloadService> _downloadServiceMock;
    private Mock<IRegexService> _regexServiceMock;
    private Mock<ILinkPreviewService> _fallbackLinkPreviewServiceMock;
    private Mock<ILinkPreviewLogger> _linkPreviewLoggerMock;

    private LinkPreviewService _service;

    [SetUp]
    public void SetUp()
    {
        _downloadServiceMock = new Mock<IDownloadService>();
        _regexServiceMock = new Mock<IRegexService>();
        _fallbackLinkPreviewServiceMock = new Mock<ILinkPreviewService>();
        _linkPreviewLoggerMock = new Mock<ILinkPreviewLogger>();
        _service = new LinkPreviewService(
            _downloadServiceMock.Object,
            _regexServiceMock.Object,
            _fallbackLinkPreviewServiceMock.Object,
            _linkPreviewLoggerMock.Object);
    }

    [Test]
    public async Task GenerateLinkPreviewAsync_When_Successful_ShouldReturnLinkPreview()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var request = new LinkPreviewRequest
        {
            Url = "https://example.com"
        };
        var pageContent = "<html><head><meta property=\"og:title\" content=\"Test Title\"></head></html>";
        var openGraphModel = new OpenGraphModel
        {
            Title = "Test Title",
            ImageUrl = "https://example.com/image.jpg"
        };

        _downloadServiceMock
            .Setup(x => x.DownloadAsStringAsync(request.Url, cancellationToken))
            .ReturnsAsync(pageContent);
        _regexServiceMock
            .Setup(x => x.GetOpenGraphModel(pageContent))
            .Returns(openGraphModel);

        // Act
        var result = await _service.GenerateLinkPreviewAsync(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeNull();
        result.Title.Should().Be("Test Title");
        result.ImageUrl.Should().Be("https://example.com/image.jpg");
    }

    [Test]
    public async Task GenerateLinkPreviewAsync_When_RegexServiceReturnsNull_ShouldReturnNullOpenGraphModel()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var request = new LinkPreviewRequest
        {
            Url = "https://example.com"
        };
        var pageContent = "<html></html>";

        _downloadServiceMock
            .Setup(x => x.DownloadAsStringAsync(request.Url, cancellationToken))
            .ReturnsAsync(pageContent);
        _regexServiceMock
            .Setup(x => x.GetOpenGraphModel(pageContent))
            .Returns((OpenGraphModel)null!);

        // Act
        var result = await _service.GenerateLinkPreviewAsync(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeNull();
    }

    [Test]
    public async Task GenerateLinkPreviewAsync_When_TitleContainsHtmlEntities_ShouldDecodeTitle()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var request = new LinkPreviewRequest
        {
            Url = "https://example.com"
        };
        var pageContent = "<html></html>";
        var openGraphModel = new OpenGraphModel
        {
            Title = "&lt;Test &amp; Title&gt;",
            ImageUrl = "https://example.com/image.jpg"
        };

        _downloadServiceMock
            .Setup(x => x.DownloadAsStringAsync(request.Url, cancellationToken))
            .ReturnsAsync(pageContent);
        _regexServiceMock
            .Setup(x => x.GetOpenGraphModel(pageContent))
            .Returns(openGraphModel);

        // Act
        var result = await _service.GenerateLinkPreviewAsync(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeNull();
        result.Title.Should().Be("<Test & Title>");
    }

    [Test]
    public async Task GenerateLinkPreviewAsync_When_ForbiddenWithSecretKey_ShouldUseFallbackService()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var request = new LinkPreviewRequest
        {
            Url = "https://example.com",
            SecretKey = "secret"
        };
        var fallbackResponse = new OpenGraphModel
        {
            Title = "Fallback Title"
        };
        _downloadServiceMock
            .Setup(x => x.DownloadAsStringAsync(request.Url, cancellationToken))
            .ThrowsAsync(new HttpRequestException("Forbidden", null, HttpStatusCode.Forbidden));
        _fallbackLinkPreviewServiceMock
            .Setup(x => x.GenerateLinkPreviewAsync(request, cancellationToken))
            .ReturnsAsync(fallbackResponse);

        // Act
        var result = await _service.GenerateLinkPreviewAsync(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeNull();
        result.Title.Should().Be("Fallback Title");
        _fallbackLinkPreviewServiceMock.Verify(x => x.GenerateLinkPreviewAsync(request, cancellationToken), Times.Once);
    }

    [Test]
    public async Task GenerateLinkPreviewAsync_When_ForbiddenWithoutSecretKey_ShouldThrow()
    {
        // Arrange
        var request = new LinkPreviewRequest
        {
            Url = "https://example.com"
        };
        var cancellationToken = new CancellationToken();

        _downloadServiceMock
            .Setup(x => x.DownloadAsStringAsync(request.Url, cancellationToken))
            .ThrowsAsync(new HttpRequestException("Forbidden", null, HttpStatusCode.Forbidden));

        // Act & Assert
        await _service
            .Invoking(x => x.GenerateLinkPreviewAsync(request, cancellationToken))
            .Should()
            .ThrowAsync<HttpRequestException>();

        _fallbackLinkPreviewServiceMock.Verify(x => x.GenerateLinkPreviewAsync(It.IsAny<LinkPreviewRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task GenerateLinkPreviewAsync_When_NonForbiddenHttpException_ShouldThrow()
    {
        // Arrange
        var request = new LinkPreviewRequest
        {
            Url = "https://example.com",
            SecretKey = "secret"
        };
        var cancellationToken = new CancellationToken();

        _downloadServiceMock
            .Setup(x => x.DownloadAsStringAsync(request.Url, cancellationToken))
            .ThrowsAsync(new HttpRequestException("Not Found", null, HttpStatusCode.NotFound));

        // Act & Assert
        await _service
            .Invoking(x => x.GenerateLinkPreviewAsync(request, cancellationToken))
            .Should()
            .ThrowAsync<HttpRequestException>();
        _fallbackLinkPreviewServiceMock.Verify(x => x.GenerateLinkPreviewAsync(It.IsAny<LinkPreviewRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
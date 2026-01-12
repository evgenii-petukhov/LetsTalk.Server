using FluentAssertions;
using LetsTalk.Server.LinkPreview.Utility.Services;

namespace LetsTalk.Server.LinkPreview.Utility.Tests;

[TestFixture]
public class RegexServiceTests
{
    private RegexService _regexService;

    [SetUp]
    public void SetUp()
    {
        _regexService = new RegexService();
    }

    [Test]
    public void GetOpenGraphModel_When_OpenGraphTitleExists_ShouldReturnOpenGraphTitle()
    {
        // Arrange
        var html = "<html><head><meta property=\"og:title\" content=\"OpenGraph Title\"></head></html>";

        // Act
        var result = _regexService.GetOpenGraphModel(html);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("OpenGraph Title");
        result.ImageUrl.Should().BeNull();
    }

    [Test]
    public void GetOpenGraphModel_When_OpenGraphTitleAndImageExist_ShouldReturnBoth()
    {
        // Arrange
        var html = "<html><head><meta property=\"og:title\" content=\"OpenGraph Title\"><meta property=\"og:image\" content=\"https://example.com/image.jpg\"></head></html>";

        // Act
        var result = _regexService.GetOpenGraphModel(html);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("OpenGraph Title");
        result.ImageUrl.Should().Be("https://example.com/image.jpg");
    }

    [Test]
    public void GetOpenGraphModel_When_NoOpenGraphTitleButHasTitle_ShouldReturnTitle()
    {
        // Arrange
        var html = "<html><head><title>Page Title</title></head></html>";

        // Act
        var result = _regexService.GetOpenGraphModel(html);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Page Title");
        result.ImageUrl.Should().BeNull();
    }

    [Test]
    public void GetOpenGraphModel_When_NoTitleAtAll_ShouldReturnNull()
    {
        // Arrange
        var html = "<html><head></head></html>";

        // Act
        var result = _regexService.GetOpenGraphModel(html);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void GetOpenGraphModel_When_EmptyOpenGraphTitle_ShouldFallbackToTitle()
    {
        // Arrange
        var html = "<html><head><meta property=\"og:title\" content=\"\"><title>Page Title</title></head></html>";

        // Act
        var result = _regexService.GetOpenGraphModel(html);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Page Title");
    }

    [Test]
    public void GetOpenGraphModel_When_WhitespaceOpenGraphTitle_ShouldFallbackToTitle()
    {
        // Arrange
        var html = "<html><head><meta property=\"og:title\" content=\"   \"><title>Page Title</title></head></html>";

        // Act
        var result = _regexService.GetOpenGraphModel(html);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Page Title");
    }

    [Test]
    [TestCase("'")]
    [TestCase("\"")]
    public void GetOpenGraphModel_When_DifferentQuoteTypes_ShouldWork(string quote)
    {
        // Arrange
        var html = $"<html><head><meta property={quote}og:title{quote} content={quote}Test Title{quote}></head></html>";

        // Act
        var result = _regexService.GetOpenGraphModel(html);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Test Title");
    }

    [Test]
    public void GetOpenGraphModel_When_ComplexHtml_ShouldExtractCorrectly()
    {
        // Arrange
        var html = @"
            <html>
            <head>
                <meta charset=""utf-8"">
                <meta property=""og:title"" content=""Complex Page Title"">
                <meta property=""og:description"" content=""Some description"">
                <meta property=""og:image"" content=""https://example.com/complex.jpg"">
                <title>Fallback Title</title>
            </head>
            <body>Content</body>
            </html>";

        // Act
        var result = _regexService.GetOpenGraphModel(html);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Complex Page Title");
        result.ImageUrl.Should().Be("https://example.com/complex.jpg");
    }

    [Test]
    public void GetOpenGraphModel_When_EmptyInput_ShouldReturnNull()
    {
        // Act
        var result = _regexService.GetOpenGraphModel("");

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void GetOpenGraphModel_When_OnlyImageNoTitle_ShouldReturnNull()
    {
        // Arrange
        var html = "<html><head><meta property=\"og:image\" content=\"https://example.com/image.jpg\"></head></html>";

        // Act
        var result = _regexService.GetOpenGraphModel(html);

        // Assert
        result.Should().BeNull();
    }
}
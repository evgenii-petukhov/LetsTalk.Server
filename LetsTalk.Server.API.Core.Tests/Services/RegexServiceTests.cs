using FluentAssertions;
using LetsTalk.Server.API.Core.Models.HtmlGenerator;
using LetsTalk.Server.API.Core.Services;

namespace LetsTalk.Server.API.Core.Tests.Services;

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
    public void ReplaceUrlsByHref_ShouldReturnOriginalText_WhenNoUrlsPresent()
    {
        // Arrange
        var input = "This is just plain text without any URLs.";

        // Act
        var result = _regexService.ReplaceUrlsByHref(input);

        // Assert
        result.Html.Should().Be(input);
        result.Url.Should().BeNull();
    }

    [Test]
    public void ReplaceUrlsByHref_ShouldReplaceHttpUrl_WithAnchorTag()
    {
        // Arrange
        var input = "Visit http://example.com for more info";
        var expectedHtml = "Visit <a href=\"http://example.com\" target=\"_blank\">http://example.com</a> for more info";

        // Act
        var result = _regexService.ReplaceUrlsByHref(input);

        // Assert
        result.Html.Should().Be(expectedHtml);
        result.Url.Should().Be("http://example.com");
    }

    [Test]
    public void ReplaceUrlsByHref_ShouldReplaceHttpsUrl_WithAnchorTag()
    {
        // Arrange
        var input = "Check out https://secure.example.com/path";
        var expectedHtml = "Check out <a href=\"https://secure.example.com/path\" target=\"_blank\">https://secure.example.com/path</a>";

        // Act
        var result = _regexService.ReplaceUrlsByHref(input);

        // Assert
        result.Html.Should().Be(expectedHtml);
        result.Url.Should().Be("https://secure.example.com/path");
    }

    [Test]
    public void ReplaceUrlsByHref_ShouldReplaceFtpUrl_WithAnchorTag()
    {
        // Arrange
        var input = "Download from ftp://files.example.com/file.zip";
        var expectedHtml = "Download from <a href=\"ftp://files.example.com/file.zip\" target=\"_blank\">ftp://files.example.com/file.zip</a>";

        // Act
        var result = _regexService.ReplaceUrlsByHref(input);

        // Assert
        result.Html.Should().Be(expectedHtml);
        result.Url.Should().Be("ftp://files.example.com/file.zip");
    }

    [Test]
    public void ReplaceUrlsByHref_ShouldReplaceMultipleUrls_WithAnchorTags()
    {
        // Arrange
        var input = "Visit http://example.com and https://another.com";
        var expectedHtml = "Visit <a href=\"http://example.com\" target=\"_blank\">http://example.com</a> and <a href=\"https://another.com\" target=\"_blank\">https://another.com</a>";

        // Act
        var result = _regexService.ReplaceUrlsByHref(input);

        // Assert
        result.Html.Should().Be(expectedHtml);
        result.Url.Should().Be("http://example.com");
    }

    [Test]
    public void ReplaceUrlsByHref_ShouldHandleUrlWithQueryParameters()
    {
        // Arrange
        var input = "Search https://example.com/search?q=test&page=1";
        var expectedHtml = "Search <a href=\"https://example.com/search?q=test&page=1\" target=\"_blank\">https://example.com/search?q=test&page=1</a>";

        // Act
        var result = _regexService.ReplaceUrlsByHref(input);

        // Assert
        result.Html.Should().Be(expectedHtml);
        result.Url.Should().Be("https://example.com/search?q=test&page=1");
    }

    [Test]
    public void ReplaceUrlsByHref_ShouldHandleUrlWithFragment()
    {
        // Arrange
        var input = "Go to https://example.com/page#section";
        var expectedHtml = "Go to <a href=\"https://example.com/page#section\" target=\"_blank\">https://example.com/page#section</a>";

        // Act
        var result = _regexService.ReplaceUrlsByHref(input);

        // Assert
        result.Html.Should().Be(expectedHtml);
        result.Url.Should().Be("https://example.com/page#section");
    }

    [Test]
    public void ReplaceUrlsByHref_ShouldReturnFirstUrl_WhenMultipleUrlsPresent()
    {
        // Arrange
        var input = "First https://first.com then https://second.com";

        // Act
        var result = _regexService.ReplaceUrlsByHref(input);

        // Assert
        result.Url.Should().Be("https://first.com");
    }

    [Test]
    public void ReplaceUrlsByHref_ShouldHandleEmptyString()
    {
        // Act
        var result = _regexService.ReplaceUrlsByHref("");

        // Assert
        result.Html.Should().Be("");
        result.Url.Should().BeNull();
    }
}
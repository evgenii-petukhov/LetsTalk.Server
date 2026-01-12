using FluentAssertions;
using LetsTalk.Server.API.Core.Abstractions;
using LetsTalk.Server.API.Core.Models.HtmlGenerator;
using LetsTalk.Server.API.Core.Services;
using Moq;

namespace LetsTalk.Server.API.Core.Tests.Services;

[TestFixture]
public class HtmlGeneratorTests
{
    private Mock<IRegexService> _regexServiceMock;
    private HtmlGenerator _htmlGenerator;

    [SetUp]
    public void SetUp()
    {
        _regexServiceMock = new Mock<IRegexService>();
        _htmlGenerator = new HtmlGenerator(_regexServiceMock.Object);
    }

    [Test]
    public void GetHtml_ShouldReturnEmptyResult_WhenTextIsNull()
    {
        // Act
        var result = _htmlGenerator.GetHtml(null);

        // Assert
        result.Should().Be(new HtmlGeneratorResult());
        _regexServiceMock.Verify(x => x.ReplaceUrlsByHref(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetHtml_ShouldReturnEmptyResult_WhenTextIsEmpty()
    {
        // Act
        var result = _htmlGenerator.GetHtml("");

        // Assert
        result.Should().Be(new HtmlGeneratorResult());
        _regexServiceMock.Verify(x => x.ReplaceUrlsByHref(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetHtml_ShouldReturnEmptyResult_WhenTextIsWhitespace()
    {
        // Act
        var result = _htmlGenerator.GetHtml("   ");

        // Assert
        result.Should().Be(new HtmlGeneratorResult());
        _regexServiceMock.Verify(x => x.ReplaceUrlsByHref(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetHtml_ShouldWrapSingleLineInParagraph()
    {
        // Arrange
        var text = "Hello world";
        var expectedHtml = "<p>Hello world</p>";
        var expectedResult = new HtmlGeneratorResult("processed html", "url");

        _regexServiceMock
            .Setup(x => x.ReplaceUrlsByHref(expectedHtml))
            .Returns(expectedResult);

        // Act
        var result = _htmlGenerator.GetHtml(text);

        // Assert
        result.Should().Be(expectedResult);
        _regexServiceMock.Verify(x => x.ReplaceUrlsByHref(expectedHtml), Times.Once);
    }

    [Test]
    public void GetHtml_ShouldWrapMultipleLinesInParagraphs_WithCarriageReturnNewline()
    {
        // Arrange
        var text = "Line 1\r\nLine 2\r\nLine 3";
        var expectedHtml = "<p>Line 1</p><p>Line 2</p><p>Line 3</p>";
        var expectedResult = new HtmlGeneratorResult("processed html", "url");

        _regexServiceMock
            .Setup(x => x.ReplaceUrlsByHref(expectedHtml))
            .Returns(expectedResult);

        // Act
        var result = _htmlGenerator.GetHtml(text);

        // Assert
        result.Should().Be(expectedResult);
        _regexServiceMock.Verify(x => x.ReplaceUrlsByHref(expectedHtml), Times.Once);
    }

    [Test]
    public void GetHtml_ShouldWrapMultipleLinesInParagraphs_WithNewlineOnly()
    {
        // Arrange
        var text = "Line 1\nLine 2\nLine 3";
        var expectedHtml = "<p>Line 1</p><p>Line 2</p><p>Line 3</p>";
        var expectedResult = new HtmlGeneratorResult("processed html", "url");

        _regexServiceMock
            .Setup(x => x.ReplaceUrlsByHref(expectedHtml))
            .Returns(expectedResult);

        // Act
        var result = _htmlGenerator.GetHtml(text);

        // Assert
        result.Should().Be(expectedResult);
        _regexServiceMock.Verify(x => x.ReplaceUrlsByHref(expectedHtml), Times.Once);
    }

    [Test]
    public void GetHtml_ShouldSkipEmptyLines()
    {
        // Arrange
        var text = "Line 1\r\n\r\nLine 2\r\n   \r\nLine 3";
        var expectedHtml = "<p>Line 1</p><p>Line 2</p><p>Line 3</p>";
        var expectedResult = new HtmlGeneratorResult("processed html", "url");

        _regexServiceMock
            .Setup(x => x.ReplaceUrlsByHref(expectedHtml))
            .Returns(expectedResult);

        // Act
        var result = _htmlGenerator.GetHtml(text);

        // Assert
        result.Should().Be(expectedResult);
        _regexServiceMock.Verify(x => x.ReplaceUrlsByHref(expectedHtml), Times.Once);
    }

    [Test]
    public void GetHtml_ShouldTrimWhitespaceFromLines()
    {
        // Arrange
        var text = "  Line 1  \r\n  Line 2  ";
        var expectedHtml = "<p>Line 1</p><p>Line 2</p>";
        var expectedResult = new HtmlGeneratorResult("processed html", "url");

        _regexServiceMock
            .Setup(x => x.ReplaceUrlsByHref(expectedHtml))
            .Returns(expectedResult);

        // Act
        var result = _htmlGenerator.GetHtml(text);

        // Assert
        result.Should().Be(expectedResult);
        _regexServiceMock.Verify(x => x.ReplaceUrlsByHref(expectedHtml), Times.Once);
    }
}
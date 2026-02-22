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
        var result = _htmlGenerator.GetHtml(null!);

        // Assert
        result.Should().Be(new HtmlGeneratorResult());
        _regexServiceMock.Verify(x => x.ReplaceUrlsByHref(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetHtml_ShouldReturnEmptyResult_WhenTextIsEmpty()
    {
        // Act
        var result = _htmlGenerator.GetHtml(string.Empty);

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
    public void GetHtml_ShouldWrapSingleLineWithoutEmojis()
    {
        // Arrange
        var inputText = "Hello world";
        var expectedHtml = $"<p>{inputText}</p>";
        string expectedUrl = null!;

        _regexServiceMock
            .Setup(x => x.ReplaceUrlsByHref(expectedHtml))
            .Returns((expectedHtml, expectedUrl));

        _regexServiceMock
            .Setup(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == inputText)))
            .Returns((inputText, 0, false));

        // Act
        var result = _htmlGenerator.GetHtml(inputText);

        // Assert
        result.Should().BeEquivalentTo(new HtmlGeneratorResult(expectedHtml, expectedUrl, false, 0));
        _regexServiceMock.Verify(x => x.ReplaceUrlsByHref(expectedHtml), Times.Once);
        _regexServiceMock.Verify(x => x.WrapEmojisWithSpan(It.IsAny<string>()), Times.Once);
        _regexServiceMock.Verify(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == inputText)), Times.Once);
    }

    [Test]
    public void GetHtml_ShouldWrapEmojisInSingleParagraphLine()
    {
        // Arrange
        var inputText = "Hello world 😀👋";
        var expectedEmojiWrap = "Hello world <span class=\"emoji\">😀</span><span class=\"emoji\">👋</span>";
        var expectedHtml = $"<p>{expectedEmojiWrap}</p>";
        string expectedUrl = null!;

        _regexServiceMock
            .Setup(x => x.ReplaceUrlsByHref(expectedHtml))
            .Returns((expectedHtml, expectedUrl));

        _regexServiceMock
            .Setup(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == inputText)))
            .Returns((expectedEmojiWrap, 2, false));

        // Act
        var result = _htmlGenerator.GetHtml(inputText);

        // Assert
        result.Should().BeEquivalentTo(new HtmlGeneratorResult(expectedHtml, expectedUrl, false, 2));
        _regexServiceMock.Verify(x => x.ReplaceUrlsByHref(expectedHtml), Times.Once);
        _regexServiceMock.Verify(x => x.WrapEmojisWithSpan(It.IsAny<string>()), Times.Once);
        _regexServiceMock.Verify(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == inputText)), Times.Once);
    }

    [Test]
    public void GetHtml_ShouldWrapSingleEmojiInSingleParagraphLine()
    {
        // Arrange
        var inputText = "❤️";
        var expectedEmojiWrap = "<span class=\"emoji\">❤️</span>";
        var expectedHtml = $"<p>{expectedEmojiWrap}</p>";
        string expectedUrl = null!;

        _regexServiceMock
            .Setup(x => x.ReplaceUrlsByHref(expectedHtml))
            .Returns((expectedHtml, expectedUrl));

        _regexServiceMock
            .Setup(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == inputText)))
            .Returns((expectedEmojiWrap, 1, true));

        // Act
        var result = _htmlGenerator.GetHtml(inputText);

        // Assert
        result.Should().BeEquivalentTo(new HtmlGeneratorResult(expectedHtml, expectedUrl, true, 1));
        _regexServiceMock.Verify(x => x.ReplaceUrlsByHref(expectedHtml), Times.Once);
        _regexServiceMock.Verify(x => x.WrapEmojisWithSpan(It.IsAny<string>()), Times.Once);
        _regexServiceMock.Verify(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == inputText)), Times.Once);
    }

    [Test]
    public void GetHtml_ShouldWrapMultipleLinesWithoutEmojis_WithCarriageReturnNewline()
    {
        // Arrange
        var line1 = "Line 1";
        var line2 = "Line 2";
        var inputText = $"{line1}\r\n{line2}";
        var expectedHtml = $"<p>{line1}</p><p>{line2}</p>";
        string expectedUrl = null!;

        _regexServiceMock
            .Setup(x => x.ReplaceUrlsByHref(expectedHtml))
            .Returns((expectedHtml, expectedUrl));

        _regexServiceMock
            .Setup(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == line1)))
            .Returns((line1, 0, false));

        _regexServiceMock
            .Setup(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == line2)))
            .Returns((line2, 0, false));

        // Act
        var result = _htmlGenerator.GetHtml(inputText);

        // Assert
        result.Should().BeEquivalentTo(new HtmlGeneratorResult(expectedHtml, expectedUrl, false, 0));
        _regexServiceMock.Verify(x => x.ReplaceUrlsByHref(expectedHtml), Times.Once);
        _regexServiceMock.Verify(x => x.WrapEmojisWithSpan(It.IsAny<string>()), Times.Exactly(2));
        _regexServiceMock.Verify(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == line1)), Times.Once);
        _regexServiceMock.Verify(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == line2)), Times.Once);
    }

    [Test]
    public void GetHtml_ShouldWrapMultipleLinesWithEmojis_WithCarriageReturnNewline()
    {
        // Arrange
        var lineOne = "Line 1😀";
        var lineOneEmojiWrap = "Line 1<span class=\"emoji\">😀</span>";
        var lineTwo = "Line 2😞";
        var lineTwoEmojiWrap = "Line 2<span class=\"emoji\">😞</span>";
        var inputText = $"{lineOne}\r\n{lineTwo}";
        var expectedHtml = $"<p>{lineOneEmojiWrap}</p><p>{lineTwoEmojiWrap}</p>";
        string expectedUrl = null!;

        _regexServiceMock
            .Setup(x => x.ReplaceUrlsByHref(expectedHtml))
            .Returns((expectedHtml, expectedUrl));

        _regexServiceMock
            .Setup(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == lineOne)))
            .Returns((lineOneEmojiWrap, 1, false));

        _regexServiceMock
            .Setup(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == lineTwo)))
            .Returns((lineTwoEmojiWrap, 1, false));

        // Act
        var result = _htmlGenerator.GetHtml(inputText);

        // Assert
        result.Should().BeEquivalentTo(new HtmlGeneratorResult(expectedHtml, expectedUrl, false, 2));
        _regexServiceMock.Verify(x => x.ReplaceUrlsByHref(expectedHtml), Times.Once);
        _regexServiceMock.Verify(x => x.WrapEmojisWithSpan(It.IsAny<string>()), Times.Exactly(2));
        _regexServiceMock.Verify(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == lineOne)), Times.Once);
        _regexServiceMock.Verify(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == lineTwo)), Times.Once);
    }

    [Test]
    public void GetHtml_ShouldWrapMultipleLinesInParagraphs_WithNewlineOnly()
    {
        // Arrange
        var line1 = "Line 1";
        var line2 = "Line 2";
        var inputText = $"{line1}\n{line2}\n";
        var expectedHtml = $"<p>{line1}</p><p>{line2}</p>";
        string expectedUrl = null!;

        _regexServiceMock
            .Setup(x => x.ReplaceUrlsByHref(expectedHtml))
            .Returns((expectedHtml, expectedUrl));

        _regexServiceMock
            .Setup(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == line1)))
            .Returns((line1, 0, false));

        _regexServiceMock
            .Setup(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == line2)))
            .Returns((line2, 0, false));

        // Act
        var result = _htmlGenerator.GetHtml(inputText);

        // Assert
        result.Should().BeEquivalentTo(new HtmlGeneratorResult(expectedHtml, expectedUrl, false, 0));
        _regexServiceMock.Verify(x => x.ReplaceUrlsByHref(expectedHtml), Times.Once);
        _regexServiceMock.Verify(x => x.WrapEmojisWithSpan(It.IsAny<string>()), Times.Exactly(2));
        _regexServiceMock.Verify(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == line1)), Times.Once);
        _regexServiceMock.Verify(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == line2)), Times.Once);
    }

    [Test]
    public void GetHtml_ShouldSkipEmptyLines()
    {
        // Arrange
        var line1 = "Line 1";
        var line2 = "Line 2";
        var inputText = $"{line1}\r\n\r\n{line2}\r\n   \r\n";
        var expectedHtml = $"<p>{line1}</p><p>{line2}</p>";
        string expectedUrl = null!;

        _regexServiceMock
            .Setup(x => x.ReplaceUrlsByHref(expectedHtml))
            .Returns((expectedHtml, expectedUrl));

        _regexServiceMock
            .Setup(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == line1)))
            .Returns((line1, 0, false));

        _regexServiceMock
            .Setup(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == line2)))
            .Returns((line2, 0, false));

        // Act
        var result = _htmlGenerator.GetHtml(inputText);

        // Assert
        result.Should().BeEquivalentTo(new HtmlGeneratorResult(expectedHtml, expectedUrl, false, 0));
        _regexServiceMock.Verify(x => x.ReplaceUrlsByHref(expectedHtml), Times.Once);
        _regexServiceMock.Verify(x => x.WrapEmojisWithSpan(It.IsAny<string>()), Times.Exactly(2));
        _regexServiceMock.Verify(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == line1)), Times.Once);
        _regexServiceMock.Verify(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == line2)), Times.Once);
    }

    [Test]
    public void GetHtml_ShouldTrimWhitespaceFromLines()
    {
        // Arrange
        var line1 = "Line 1";
        var line2 = "Line 2";
        var text = $"  {line1}  \r\n  {line2}  ";
        var expectedHtml = $"<p>{line1}</p><p>{line2}</p>";
        string expectedUrl = null!;

        _regexServiceMock
            .Setup(x => x.ReplaceUrlsByHref(expectedHtml))
            .Returns((expectedHtml, expectedUrl));

        _regexServiceMock
            .Setup(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == line1)))
            .Returns((line1, 0, false));

        _regexServiceMock
            .Setup(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == line2)))
            .Returns((line2, 0, false));

        // Act
        var result = _htmlGenerator.GetHtml(text);

        // Assert
        result.Should().BeEquivalentTo(new HtmlGeneratorResult(expectedHtml, expectedUrl, false, 0));
        _regexServiceMock.Verify(x => x.ReplaceUrlsByHref(expectedHtml), Times.Once);
        _regexServiceMock.Verify(x => x.WrapEmojisWithSpan(It.IsAny<string>()), Times.Exactly(2));
        _regexServiceMock.Verify(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == line1)), Times.Once);
        _regexServiceMock.Verify(x => x.WrapEmojisWithSpan(It.Is<string>(x => x == line2)), Times.Once);
    }
}
using FluentAssertions;
using LetsTalk.Server.Persistence.EntityFramework.Repository.Abstractions;
using Moq;

namespace LetsTalk.Server.Persistence.EntityFramework.Services.Tests;

[TestFixture]
public class LinkPreviewEntityFrameworkServiceTests
{
    private Mock<ILinkPreviewRepository> _mockLinkPreviewRepository;
    private LinkPreviewEntityFrameworkService _service;

    [SetUp]
    public void SetUp()
    {
        _mockLinkPreviewRepository = new Mock<ILinkPreviewRepository>();
        _service = new LinkPreviewEntityFrameworkService(_mockLinkPreviewRepository.Object);
    }

    [TestFixture]
    public class GetIdByUrlAsyncTests : LinkPreviewEntityFrameworkServiceTests
    {
        [Test]
        public async Task GetIdByUrlAsync_WithValidUrl_ShouldReturnIdAsString()
        {
            // Arrange
            const string url = "https://example.com";
            const int linkPreviewId = 123;
            const string expectedResult = "123";

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(linkPreviewId);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().Be(expectedResult);
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WhenLinkPreviewNotFound_ShouldReturnNull()
        {
            // Arrange
            const string url = "https://nonexistent.com";
            const int linkPreviewId = 0;

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(linkPreviewId);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().BeNull();
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithCancellationToken_ShouldPassTokenToRepository()
        {
            // Arrange
            const string url = "https://example.com";
            const int linkPreviewId = 456;
            var cancellationToken = new CancellationToken();

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, cancellationToken))
                .ReturnsAsync(linkPreviewId);

            // Act
            await _service.GetIdByUrlAsync(url, cancellationToken);

            // Assert
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, cancellationToken), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithEmptyUrl_ShouldPassEmptyUrlToRepository()
        {
            // Arrange
            const string url = "";
            const int linkPreviewId = 0;

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(linkPreviewId);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().BeNull();
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithNullUrl_ShouldPassNullUrlToRepository()
        {
            // Arrange
            string url = null;
            const int linkPreviewId = 0;

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(linkPreviewId);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().BeNull();
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithLargeId_ShouldReturnIdAsString()
        {
            // Arrange
            const string url = "https://example.com/large-id";
            const int linkPreviewId = int.MaxValue;
            var expectedResult = int.MaxValue.ToString();

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(linkPreviewId);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().Be(expectedResult);
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithNegativeId_ShouldReturnIdAsString()
        {
            // Arrange
            const string url = "https://example.com/negative";
            const int linkPreviewId = -1;
            const string expectedResult = "-1";

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(linkPreviewId);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().Be(expectedResult);
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithComplexUrl_ShouldReturnIdAsString()
        {
            // Arrange
            const string url = "https://example.com/path/to/resource?param1=value1&param2=value2#fragment";
            const int linkPreviewId = 789;
            const string expectedResult = "789";

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(linkPreviewId);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().Be(expectedResult);
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithHttpUrl_ShouldReturnIdAsString()
        {
            // Arrange
            const string url = "http://example.com";
            const int linkPreviewId = 101;
            const string expectedResult = "101";

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(linkPreviewId);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().Be(expectedResult);
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithInvalidUrl_ShouldPassToRepositoryAndHandleResponse()
        {
            // Arrange
            const string url = "not-a-valid-url";
            const int linkPreviewId = 0;

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(linkPreviewId);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().BeNull();
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithWhitespaceUrl_ShouldPassToRepositoryAndHandleResponse()
        {
            // Arrange
            const string url = "   ";
            const int linkPreviewId = 0;

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(linkPreviewId);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().BeNull();
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithVeryLongUrl_ShouldReturnIdAsString()
        {
            // Arrange
            var url = "https://example.com/" + new string('a', 1000);
            const int linkPreviewId = 999;
            const string expectedResult = "999";

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(linkPreviewId);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().Be(expectedResult);
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithSpecialCharactersInUrl_ShouldReturnIdAsString()
        {
            // Arrange
            const string url = "https://example.com/path with spaces/åäö/中文/🚀";
            const int linkPreviewId = 555;
            const string expectedResult = "555";

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(linkPreviewId);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().Be(expectedResult);
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            const string url = "https://example.com";
            var expectedException = new InvalidOperationException("Database error");

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var act = async () => await _service.GetIdByUrlAsync(url);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Database error");

            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WhenCancellationRequested_ShouldPropagateOperationCanceledException()
        {
            // Arrange
            const string url = "https://example.com";
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            var cancellationToken = cancellationTokenSource.Token;

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, cancellationToken))
                .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            var act = async () => await _service.GetIdByUrlAsync(url, cancellationToken);
            await act.Should().ThrowAsync<OperationCanceledException>();

            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, cancellationToken), Times.Once);
        }
    }
}
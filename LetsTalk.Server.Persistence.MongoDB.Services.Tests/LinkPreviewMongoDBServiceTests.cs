using FluentAssertions;
using LetsTalk.Server.Persistence.MongoDB.Repository.Abstractions;
using Moq;

namespace LetsTalk.Server.Persistence.MongoDB.Services.Tests;

[TestFixture]
public class LinkPreviewMongoDBServiceTests
{
    private Mock<ILinkPreviewRepository> _mockLinkPreviewRepository;
    private LinkPreviewMongoDBService _service;

    [SetUp]
    public void SetUp()
    {
        _mockLinkPreviewRepository = new Mock<ILinkPreviewRepository>();
        _service = new LinkPreviewMongoDBService(_mockLinkPreviewRepository.Object);
    }

    [TestFixture]
    public class GetIdByUrlAsyncTests : LinkPreviewMongoDBServiceTests
    {
        [Test]
        public async Task GetIdByUrlAsync_WithValidUrl_ShouldReturnId()
        {
            // Arrange
            const string url = "https://example.com/article";
            const string expectedId = "507f1f77bcf86cd799439011";

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().Be(expectedId);
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithUrlNotFound_ShouldReturnNull()
        {
            // Arrange
            const string url = "https://nonexistent.com/article";

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync((string)null);

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
            const string url = "https://example.com/test";
            const string expectedId = "507f1f77bcf86cd799439012";
            var cancellationToken = new CancellationToken();

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, cancellationToken))
                .ReturnsAsync(expectedId);

            // Act
            await _service.GetIdByUrlAsync(url, cancellationToken);

            // Assert
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, cancellationToken), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithNullUrl_ShouldPassNullToRepository()
        {
            // Arrange
            string url = null;

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync((string)null);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().BeNull();
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithEmptyUrl_ShouldPassEmptyStringToRepository()
        {
            // Arrange
            const string url = "";

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync((string)null);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().BeNull();
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithWhitespaceUrl_ShouldPassWhitespaceToRepository()
        {
            // Arrange
            const string url = "   ";

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync((string)null);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().BeNull();
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithHttpUrl_ShouldReturnId()
        {
            // Arrange
            const string url = "http://example.com/article";
            const string expectedId = "507f1f77bcf86cd799439013";

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().Be(expectedId);
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithHttpsUrl_ShouldReturnId()
        {
            // Arrange
            const string url = "https://secure.example.com/article";
            const string expectedId = "507f1f77bcf86cd799439014";

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().Be(expectedId);
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithUrlWithQueryParameters_ShouldReturnId()
        {
            // Arrange
            const string url = "https://example.com/article?id=123&category=tech";
            const string expectedId = "507f1f77bcf86cd799439015";

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().Be(expectedId);
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithUrlWithFragment_ShouldReturnId()
        {
            // Arrange
            const string url = "https://example.com/article#section1";
            const string expectedId = "507f1f77bcf86cd799439016";

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().Be(expectedId);
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithUrlWithPort_ShouldReturnId()
        {
            // Arrange
            const string url = "https://example.com:8080/article";
            const string expectedId = "507f1f77bcf86cd799439017";

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().Be(expectedId);
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithUrlWithSpecialCharacters_ShouldReturnId()
        {
            // Arrange
            const string url = "https://example.com/article-with-special_chars%20and%20spaces";
            const string expectedId = "507f1f77bcf86cd799439018";

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().Be(expectedId);
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithInvalidUrl_ShouldPassInvalidUrlToRepository()
        {
            // Arrange
            const string url = "not-a-valid-url";

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync((string)null);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().BeNull();
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithLongUrl_ShouldReturnId()
        {
            // Arrange
            var url = "https://example.com/" + new string('a', 1000) + "/article";
            const string expectedId = "507f1f77bcf86cd799439019";

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().Be(expectedId);
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithSubdomain_ShouldReturnId()
        {
            // Arrange
            const string url = "https://blog.subdomain.example.com/article";
            const string expectedId = "507f1f77bcf86cd799439020";

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().Be(expectedId);
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithDifferentTlds_ShouldReturnId()
        {
            // Arrange
            const string url = "https://example.co.uk/article";
            const string expectedId = "507f1f77bcf86cd799439021";

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().Be(expectedId);
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithCaseSensitiveUrl_ShouldReturnId()
        {
            // Arrange
            const string url = "https://Example.COM/Article";
            const string expectedId = "507f1f77bcf86cd799439022";

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _service.GetIdByUrlAsync(url);

            // Assert
            result.Should().Be(expectedId);
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithRepositoryException_ShouldPropagateException()
        {
            // Arrange
            const string url = "https://example.com/article";
            var expectedException = new InvalidOperationException("Repository error");

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var act = async () => await _service.GetIdByUrlAsync(url);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Repository error");

            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetIdByUrlAsync_WithCancellationRequested_ShouldPropagateOperationCanceledException()
        {
            // Arrange
            const string url = "https://example.com/article";
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

        [Test]
        public async Task GetIdByUrlAsync_WithMultipleConcurrentCalls_ShouldCallRepositoryForEach()
        {
            // Arrange
            const string url1 = "https://example1.com/article";
            const string url2 = "https://example2.com/article";
            const string expectedId1 = "507f1f77bcf86cd799439023";
            const string expectedId2 = "507f1f77bcf86cd799439024";

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId1);

            _mockLinkPreviewRepository
                .Setup(x => x.GetIdByUrlAsync(url2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId2);

            // Act
            var task1 = _service.GetIdByUrlAsync(url1);
            var task2 = _service.GetIdByUrlAsync(url2);
            var results = await Task.WhenAll(task1, task2);

            // Assert
            results[0].Should().Be(expectedId1);
            results[1].Should().Be(expectedId2);
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url1, It.IsAny<CancellationToken>()), Times.Once);
            _mockLinkPreviewRepository.Verify(x => x.GetIdByUrlAsync(url2, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
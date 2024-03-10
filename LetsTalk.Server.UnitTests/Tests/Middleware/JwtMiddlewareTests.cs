using FluentAssertions;
using LetsTalk.Server.API.Middleware;
using LetsTalk.Server.Authentication.Abstractions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace LetsTalk.Server.UnitTests.Tests.Middleware;

[TestFixture]
public class JwtMiddlewareTests
{
    private const string AuthorizationHeaderKey = "Authorization";
    private const string SampleToken = "token";
    private const string AuthorizationHeaderValue = $"Bearer {SampleToken}";
    private const string AccountIdKey = "AccountId";
    private const string AccountIdValue = "1";

    private JwtMiddleware _jwtMiddleware;
    private Mock<RequestDelegate> _mockRequestDelegate;
    private Mock<IAuthenticationClient> _mockAuthenticationClient;

    [SetUp]
    public void SetUp()
    {
        _mockRequestDelegate = new Mock<RequestDelegate>();
        _mockAuthenticationClient = new Mock<IAuthenticationClient>();
        _jwtMiddleware = new JwtMiddleware(_mockRequestDelegate.Object);
    }

    [Test]
    public async Task InvokeAsync_When_TokenIsValid_Should_AppendContextByAccountId()
    {
        // Arrange
        HttpContext ctx = new DefaultHttpContext();
        ctx.Request.Headers[AuthorizationHeaderKey] = AuthorizationHeaderValue;

        _mockAuthenticationClient
            .Setup(x => x.ValidateJwtTokenAsync(SampleToken))
            .ReturnsAsync(AccountIdValue);

        // Act
        await _jwtMiddleware.InvokeAsync(ctx, _mockAuthenticationClient.Object);

        // Assert
        ctx.Items.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { AccountIdKey, AccountIdValue }
        });
        _mockRequestDelegate.Verify(x => x.Invoke(ctx), Times.Once);
    }

    [Test]
    public async Task InvokeAsync_When_TokenIsInvalidNonEmptyString_Should_AppendContextByAccountId()
    {
        // Arrange
        HttpContext ctx = new DefaultHttpContext();
        ctx.Request.Headers[AuthorizationHeaderKey] = AuthorizationHeaderValue;

        // Act
        Func<Task> f = async () => await _jwtMiddleware.InvokeAsync(ctx, _mockAuthenticationClient.Object);

        // Assert
        await f.Should().ThrowAsync<UnauthorizedAccessException>();
        _mockRequestDelegate.Verify(x => x.Invoke(ctx), Times.Never);
    }

    [Test]
    public async Task InvokeAsync_When_TokenIsNullOrEmptyString_Should_AppendContextByAccountId()
    {
        // Arrange
        HttpContext ctx = new DefaultHttpContext();

        // Act
        await _jwtMiddleware.InvokeAsync(ctx, _mockAuthenticationClient.Object);

        // Assert
        ctx.Items.Should().BeEmpty();
        _mockRequestDelegate.Verify(x => x.Invoke(ctx), Times.Once);
    }
}

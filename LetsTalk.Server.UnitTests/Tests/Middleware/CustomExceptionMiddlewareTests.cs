using FluentAssertions;
using LetsTalk.Server.API.Middleware;
using LetsTalk.Server.Exceptions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Net;

namespace LetsTalk.Server.UnitTests.Tests.Middleware;

[TestFixture]
public class CustomExceptionMiddlewareTests
{
    private CustomExceptionMiddleware _customExceptionMiddleware;
    private Mock<RequestDelegate> _mockRequestDelegate;

    [SetUp]
    public void SetUp()
    {
        _mockRequestDelegate = new Mock<RequestDelegate>();
        _customExceptionMiddleware = new CustomExceptionMiddleware(_mockRequestDelegate.Object);
    }

    [Test]
    public async Task InvokeAsync_BadRequestException()
    {
        // Arrange
        HttpContext ctx = new DefaultHttpContext();

        _mockRequestDelegate
            .Setup(x => x.Invoke(ctx))
            .ThrowsAsync(new BadRequestException());

        // Act
        await _customExceptionMiddleware.InvokeAsync(ctx);

        // Assert
        ctx.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task InvokeAsync_NotFoundException()
    {
        // Arrange
        HttpContext ctx = new DefaultHttpContext();

        _mockRequestDelegate
            .Setup(x => x.Invoke(ctx))
            .ThrowsAsync(new NotFoundException());

        // Act
        await _customExceptionMiddleware.InvokeAsync(ctx);

        // Assert
        ctx.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Test]
    public async Task InvokeAsync_UnauthorizedAccessException()
    {
        // Arrange
        HttpContext ctx = new DefaultHttpContext();

        _mockRequestDelegate
            .Setup(x => x.Invoke(ctx))
            .ThrowsAsync(new UnauthorizedAccessException());

        // Act
        await _customExceptionMiddleware.InvokeAsync(ctx);

        // Assert
        ctx.Response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task InvokeAsync_Exception()
    {
        // Arrange
        HttpContext ctx = new DefaultHttpContext();

        _mockRequestDelegate
            .Setup(x => x.Invoke(ctx))
            .ThrowsAsync(new Exception());

        // Act
        await _customExceptionMiddleware.InvokeAsync(ctx);

        // Assert
        ctx.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }
}

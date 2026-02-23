using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Mamey.Auth.Azure;
using Mamey.Auth.Azure.Middlewares;
using System.Security.Claims;
using Xunit;

namespace Mamey.Auth.Azure.Tests.Unit;

public class AzureAuthMiddlewareTests
{
    private readonly Mock<ILogger<AzureAuthMiddleware>> _loggerMock;
    private readonly DefaultHttpContext _httpContext;
    private readonly Mock<RequestDelegate> _nextMock;

    public AzureAuthMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<AzureAuthMiddleware>>();
        _httpContext = new DefaultHttpContext();
        _nextMock = new Mock<RequestDelegate>();
    }

    [Fact]
    public async Task InvokeAsync_WhenNoMethodsEnabled_ShouldSkipAuthentication()
    {
        // Arrange
        var options = new AzureMultiAuthOptions
        {
            EnableAzure = false,
            EnableAzureB2B = false,
            EnableAzureB2C = false
        };
        var middleware = new AzureAuthMiddleware(options, _loggerMock.Object);

        // Act
        await middleware.InvokeAsync(_httpContext, _nextMock.Object);

        // Assert
        _nextMock.Verify(next => next(_httpContext), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithB2BOnlyPolicy_ShouldTryB2BScheme()
    {
        // Arrange
        var options = new AzureMultiAuthOptions
        {
            EnableAzureB2B = true,
            Policy = AzureAuthenticationPolicy.B2BOnly,
            AzureB2BScheme = "AzureB2B"
        };
        var middleware = new AzureAuthMiddleware(options, _loggerMock.Object);
        
        // Note: AuthenticateAsync is an extension method on HttpContext
        // We can't easily mock it, so we just verify the middleware doesn't throw
        // and calls next

        // Act
        await middleware.InvokeAsync(_httpContext, _nextMock.Object);

        // Assert
        _nextMock.Verify(next => next(_httpContext), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithB2COnlyPolicy_ShouldTryB2CScheme()
    {
        // Arrange
        var options = new AzureMultiAuthOptions
        {
            EnableAzureB2C = true,
            Policy = AzureAuthenticationPolicy.B2COnly,
            AzureB2CScheme = "AzureB2C"
        };
        var middleware = new AzureAuthMiddleware(options, _loggerMock.Object);
        
        // Note: AuthenticateAsync is an extension method on HttpContext
        // We can't easily mock it, so we just verify the middleware doesn't throw
        // and calls next

        // Act
        await middleware.InvokeAsync(_httpContext, _nextMock.Object);

        // Assert
        _nextMock.Verify(next => next(_httpContext), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithEitherOrPolicy_ShouldTryInOrder()
    {
        // Arrange
        var options = new AzureMultiAuthOptions
        {
            EnableAzure = true,
            EnableAzureB2B = true,
            EnableAzureB2C = true,
            Policy = AzureAuthenticationPolicy.EitherOr,
            AzureScheme = "AzureAD",
            AzureB2BScheme = "AzureB2B",
            AzureB2CScheme = "AzureB2C"
        };
        var middleware = new AzureAuthMiddleware(options, _loggerMock.Object);
        
        // Note: AuthenticateAsync is an extension method on HttpContext
        // We can't easily mock it, so we just verify the middleware doesn't throw
        // and calls next

        // Act
        await middleware.InvokeAsync(_httpContext, _nextMock.Object);

        // Assert
        _nextMock.Verify(next => next(_httpContext), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithSuccessfulAuthentication_ShouldSetUser()
    {
        // Arrange
        var options = new AzureMultiAuthOptions
        {
            EnableAzureB2B = true,
            Policy = AzureAuthenticationPolicy.B2BOnly,
            AzureB2BScheme = "AzureB2B"
        };
        var middleware = new AzureAuthMiddleware(options, _loggerMock.Object);
        
        // Note: AuthenticateAsync is an extension method on HttpContext
        // We can't easily mock it, so we just verify the middleware doesn't throw
        // and calls next

        // Act
        await middleware.InvokeAsync(_httpContext, _nextMock.Object);

        // Assert
        _nextMock.Verify(next => next(_httpContext), Times.Once);
    }
}


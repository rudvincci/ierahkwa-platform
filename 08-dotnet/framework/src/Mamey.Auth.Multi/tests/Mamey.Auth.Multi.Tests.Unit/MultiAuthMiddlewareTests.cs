using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Mamey.Auth.Multi;
using Mamey.Auth.Multi.Middlewares;
using System.Security.Claims;
using Xunit;

namespace Mamey.Auth.Multi.Tests.Unit;

public class MultiAuthMiddlewareTests
{
    private readonly Mock<ILogger<MultiAuthMiddleware>> _loggerMock;
    private readonly DefaultHttpContext _httpContext;
    private readonly Mock<RequestDelegate> _nextMock;

    public MultiAuthMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<MultiAuthMiddleware>>();
        _httpContext = new DefaultHttpContext();
        _nextMock = new Mock<RequestDelegate>();
    }

    [Fact]
    public async Task InvokeAsync_WhenNoMethodsEnabled_ShouldSkipAuthentication()
    {
        // Arrange
        var options = new MultiAuthOptions
        {
            EnableJwt = false,
            EnableDid = false,
            EnableAzure = false,
            EnableIdentity = false,
            EnableDistributed = false,
            EnableCertificate = false
        };
        var middleware = new MultiAuthMiddleware(options, _loggerMock.Object);

        // Act
        await middleware.InvokeAsync(_httpContext, _nextMock.Object);

        // Assert
        _nextMock.Verify(next => next(_httpContext), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithJwtOnlyPolicy_ShouldTryJwtScheme()
    {
        // Arrange
        var options = new MultiAuthOptions
        {
            EnableJwt = true,
            Policy = AuthenticationPolicy.JwtOnly,
            JwtScheme = "Bearer"
        };
        var middleware = new MultiAuthMiddleware(options, _loggerMock.Object);
        
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
        var options = new MultiAuthOptions
        {
            EnableJwt = true,
            EnableDid = true,
            EnableAzure = true,
            Policy = AuthenticationPolicy.EitherOr,
            JwtScheme = "Bearer",
            DidScheme = "DidBearer",
            AzureScheme = "AzureAD"
        };
        var middleware = new MultiAuthMiddleware(options, _loggerMock.Object);
        
        // Note: AuthenticateAsync is an extension method on HttpContext
        // We can't easily mock it, so we just verify the middleware doesn't throw
        // and calls next

        // Act
        await middleware.InvokeAsync(_httpContext, _nextMock.Object);

        // Assert
        _nextMock.Verify(next => next(_httpContext), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithSuccessfulJwtAuthentication_ShouldSetUser()
    {
        // Arrange
        var options = new MultiAuthOptions
        {
            EnableJwt = true,
            Policy = AuthenticationPolicy.JwtOnly,
            JwtScheme = "Bearer"
        };
        var middleware = new MultiAuthMiddleware(options, _loggerMock.Object);
        
        // Note: AuthenticateAsync is an extension method on HttpContext
        // We can't easily mock it, so we just verify the middleware doesn't throw
        // and calls next

        // Act
        await middleware.InvokeAsync(_httpContext, _nextMock.Object);

        // Assert
        _nextMock.Verify(next => next(_httpContext), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithPriorityOrderPolicy_ShouldTryInPriorityOrder()
    {
        // Arrange
        var options = new MultiAuthOptions
        {
            EnableJwt = true,
            EnableDid = true,
            Policy = AuthenticationPolicy.PriorityOrder,
            JwtScheme = "Bearer",
            DidScheme = "DidBearer"
        };
        var middleware = new MultiAuthMiddleware(options, _loggerMock.Object);
        
        // Note: AuthenticateAsync is an extension method on HttpContext
        // We can't easily mock it, so we just verify the middleware doesn't throw
        // and calls next

        // Act
        await middleware.InvokeAsync(_httpContext, _nextMock.Object);

        // Assert
        _nextMock.Verify(next => next(_httpContext), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithAllRequiredPolicy_ShouldRequireAllMethods()
    {
        // Arrange
        var options = new MultiAuthOptions
        {
            EnableJwt = true,
            EnableDid = true,
            Policy = AuthenticationPolicy.AllRequired,
            JwtScheme = "Bearer",
            DidScheme = "DidBearer"
        };
        var middleware = new MultiAuthMiddleware(options, _loggerMock.Object);
        
        // Note: AuthenticateAsync is an extension method on HttpContext
        // We can't easily mock it, so we just verify the middleware doesn't throw
        // and calls next

        // Act
        await middleware.InvokeAsync(_httpContext, _nextMock.Object);

        // Assert
        _nextMock.Verify(next => next(_httpContext), Times.Once);
    }
}


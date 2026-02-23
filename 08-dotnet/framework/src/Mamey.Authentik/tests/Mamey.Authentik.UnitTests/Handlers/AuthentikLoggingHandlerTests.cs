using System.Net;
using System.Net.Http;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Mamey.Authentik.Handlers;
using Mamey.Authentik.UnitTests.Mocks;
using Xunit;

namespace Mamey.Authentik.UnitTests.Handlers;

public class AuthentikLoggingHandlerTests : TestBase
{
    [Fact]
    public async Task SendAsync_WithSuccessResponse_LogsRequestAndResponse()
    {
        // Arrange
        var options = MockAuthentikOptions.Create();
        var loggerMock = new Mock<ILogger<AuthentikLoggingHandler>>();
        
        // Setup IsEnabled to return true so logging actually happens
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        
        var handler = new AuthentikLoggingHandler(options, loggerMock.Object)
        {
            InnerHandler = new TestHandler(HttpStatusCode.OK)
        };

        var request = new HttpRequestMessage(HttpMethod.Get, "https://test.authentik.local/api/v3/core/users/");
        var invoker = new HttpMessageInvoker(handler);

        // Act
        var response = await invoker.SendAsync(request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        // Verify logging was called (at least once for request and once for response)
        loggerMock.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task SendAsync_WithException_LogsException()
    {
        // Arrange
        var options = MockAuthentikOptions.Create();
        var loggerMock = new Mock<ILogger<AuthentikLoggingHandler>>();
        
        // Setup IsEnabled to return true so logging actually happens
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        
        var handler = new AuthentikLoggingHandler(options, loggerMock.Object)
        {
            InnerHandler = new ExceptionHandler()
        };

        var request = new HttpRequestMessage(HttpMethod.Get, "https://test.authentik.local/api/v3/core/users/");
        var invoker = new HttpMessageInvoker(handler);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => invoker.SendAsync(request, CancellationToken.None));
        
        // Verify error logging was called
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.AtLeastOnce);
    }

    private class TestHandler : DelegatingHandler
    {
        private readonly HttpStatusCode _statusCode;

        public TestHandler(HttpStatusCode statusCode)
        {
            _statusCode = statusCode;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(_statusCode));
        }
    }

    private class ExceptionHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            throw new HttpRequestException("Test exception");
        }
    }
}

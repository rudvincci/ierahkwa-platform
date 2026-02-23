using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Mamey.Authentik.Exceptions;
using Mamey.Authentik.Handlers;
using Mamey.Authentik.UnitTests.Mocks;
using Xunit;

namespace Mamey.Authentik.UnitTests.Handlers;

public class AuthentikErrorHandlerTests : TestBase
{
    [Theory]
    [InlineData(HttpStatusCode.BadRequest, typeof(AuthentikValidationException))]
    [InlineData(HttpStatusCode.Unauthorized, typeof(AuthentikAuthenticationException))]
    [InlineData(HttpStatusCode.Forbidden, typeof(AuthentikAuthorizationException))]
    [InlineData(HttpStatusCode.NotFound, typeof(AuthentikNotFoundException))]
    [InlineData(HttpStatusCode.TooManyRequests, typeof(AuthentikRateLimitException))]
    [InlineData(HttpStatusCode.InternalServerError, typeof(AuthentikApiException))]
    public async Task SendAsync_WithErrorStatusCode_ThrowsCorrectException(
        HttpStatusCode statusCode,
        Type expectedExceptionType)
    {
        // Arrange
        var logger = CreateMockLogger<AuthentikErrorHandler>();
        var handler = new AuthentikErrorHandler(logger)
        {
            InnerHandler = new ErrorResponseHandler(statusCode)
        };

        var request = new HttpRequestMessage(HttpMethod.Get, "https://test.authentik.local/api/v3/core/users/");
        var invoker = new HttpMessageInvoker(handler);

        // Act
        var act = async () => await invoker.SendAsync(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AuthentikApiException>()
            .Where(ex => ex.GetType() == expectedExceptionType);
    }

    private class ErrorResponseHandler : DelegatingHandler
    {
        private readonly HttpStatusCode _statusCode;

        public ErrorResponseHandler(HttpStatusCode statusCode)
        {
            _statusCode = statusCode;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent("{\"detail\":\"Error message\"}")
            });
        }
    }
}

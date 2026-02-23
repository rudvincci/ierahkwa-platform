using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;

namespace Mamey.Authentik.UnitTests.Mocks;

/// <summary>
/// Factory for creating mock HTTP clients for testing.
/// </summary>
public static class MockHttpClientFactory
{
    /// <summary>
    /// Creates a mock HTTP client factory.
    /// </summary>
    public static IHttpClientFactory CreateFactory(MockHttpMessageHandler handler)
    {
        var mockFactory = new Mock<IHttpClientFactory>();
        mockFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(() =>
            {
                var client = handler.ToHttpClient();
                client.BaseAddress = new Uri("https://test.authentik.local/");
                return client;
            });
        return mockFactory.Object;
    }

    /// <summary>
    /// Creates a mock HTTP message handler.
    /// </summary>
    public static MockHttpMessageHandler CreateHandler()
    {
        return new MockHttpMessageHandler();
    }

    /// <summary>
    /// Creates a JSON response content.
    /// </summary>
    public static StringContent CreateJsonContent<T>(T obj)
    {
        var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    /// <summary>
    /// Creates an error response content.
    /// </summary>
    public static StringContent CreateErrorContent(string message, Dictionary<string, string[]>? errors = null)
    {
        var errorObj = new
        {
            detail = message,
            errors = errors
        };
        var json = JsonSerializer.Serialize(errorObj, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}

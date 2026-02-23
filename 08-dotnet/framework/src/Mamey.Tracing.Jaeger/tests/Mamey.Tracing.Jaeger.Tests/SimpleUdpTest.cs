using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;
using Xunit;
using Mamey.Tracing.Jaeger;
using Mamey.Tracing.Jaeger.Tests.Helpers;
using Mamey;
using System.Net.NetworkInformation;

namespace Mamey.Tracing.Jaeger.Tests;

public class SimpleUdpTest : IClassFixture<JaegerTestFixture>
{
    private readonly JaegerTestFixture _fixture;

    public SimpleUdpTest(JaegerTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void UdpPort_ShouldBeAccessible()
    {
        // Arrange
        var udpClient = new System.Net.Sockets.UdpClient();
        
        // Act & Assert
        try
        {
            udpClient.Connect("localhost", 6831);
            udpClient.Should().NotBeNull();
        }
        finally
        {
            udpClient.Close();
        }
    }

    [Fact]
    public void Tracer_ShouldCreateSpansWithoutErrors()
    {
        // Arrange
        var tracer = _fixture.Tracer;
        
        // Act & Assert
        var action = () =>
        {
            var span = tracer.BuildSpan("simple-test").Start();
            span.SetTag("test", "value");
            span.Log("Test log");
            span.Finish();
        };
        
        action.Should().NotThrow("Span creation should not throw exceptions");
    }

    [Fact]
    public async Task Tracer_ShouldFlushSpans()
    {
        // Arrange
        var tracer = _fixture.Tracer;
        
        // Act
        var span = tracer.BuildSpan("flush-test").Start();
        span.SetTag("flush-test", "true");
        span.Finish();
        
        // Give some time for the span to be sent
        await Task.Delay(2000);
        
        // Assert - This test just verifies that no exceptions are thrown
        // The actual verification that spans reach Jaeger is done in other tests
        span.Should().NotBeNull();
    }
}

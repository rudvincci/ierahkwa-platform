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

public class ComprehensiveJaegerTest : IClassFixture<JaegerTestFixture>
{
    private readonly JaegerTestFixture _fixture;

    public ComprehensiveJaegerTest(JaegerTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Tracer_ShouldSendTracesToJaeger()
    {
        // Arrange
        const string serviceName = "test-service";
        var tracer = _fixture.Tracer;
        
        // Act - Create multiple spans to increase chances of detection
        for (int i = 0; i < 5; i++)
        {
            var span = tracer.BuildSpan($"comprehensive-test-{i}").Start();
            span.SetTag("test-number", i);
            span.SetTag("service", serviceName);
            span.Log($"Test log message {i}");
            span.Finish();
        }
        
        // Flush the tracer to ensure spans are sent to Jaeger
        await _fixture.FlushTracerAsync();
        
        // Give more time for traces to be sent and processed
        await Task.Delay(5000);
        
        // Check if service appears in services list
        var services = await _fixture.GetServicesAsync();
        services.Should().Contain(serviceName, "Service should appear in Jaeger services list");
        
        // Check if traces appear
        var traces = await _fixture.GetTracesAsync(serviceName);
        traces.Should().NotBeNull("Traces should be found");
        traces.Should().NotBeEmpty("Traces should contain data");
        
        // Verify trace content
        var testTrace = traces!.FirstOrDefault(t => t.Spans.Any(s => s.OperationName.StartsWith("comprehensive-test")));
        testTrace.Should().NotBeNull("Test trace should be found");
    }

    [Fact]
    public async Task Tracer_ShouldHandleMultipleSpansInTrace()
    {
        // Arrange
        const string serviceName = "test-service";
        var tracer = _fixture.Tracer;
        
        // Act - Create a trace with parent-child spans
        var rootSpan = tracer.BuildSpan("root-operation").Start();
        rootSpan.SetTag("service", serviceName);
        rootSpan.SetTag("operation-type", "root");
        
        var childSpan = tracer.BuildSpan("child-operation").Start();
        childSpan.SetTag("service", serviceName);
        childSpan.SetTag("operation-type", "child");
        childSpan.SetTag("parent", "root-operation");
        
        await Task.Delay(100); // Simulate work
        
        childSpan.Finish();
        rootSpan.Finish();
        
        // Wait for traces to be sent
        await Task.Delay(3000);
        
        // Assert
        var traces = await _fixture.GetTracesAsync(serviceName);
        traces.Should().NotBeNull("Traces should be found");
        traces.Should().NotBeEmpty("Traces should contain data");
        
        var testTrace = traces!.FirstOrDefault(t => t.Spans.Any(s => s.OperationName == "root-operation"));
        testTrace.Should().NotBeNull("Root operation trace should be found");
        
        var childSpanData = testTrace!.Spans.FirstOrDefault(s => s.OperationName == "child-operation");
        childSpanData.Should().NotBeNull("Child operation span should be found");
    }

    [Fact]
    public void Tracer_ShouldBeProperlyConfigured()
    {
        // Arrange & Act
        var tracer = _fixture.Tracer;
        
        // Assert
        tracer.Should().NotBeNull("Tracer should be created");
        // Note: We can't easily verify the tracer type without accessing internal classes
        
        // Verify tracer can create spans
        var span = tracer.BuildSpan("configuration-test").Start();
        span.Should().NotBeNull("Span should be created");
        span.Finish();
    }
}

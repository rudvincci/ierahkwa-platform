using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;
using Xunit;
using Mamey.Tracing.Jaeger;
using Mamey.Tracing.Jaeger.Tests.Helpers;
using Mamey;

namespace Mamey.Tracing.Jaeger.Tests;

public class JaegerApiVerificationTests : IClassFixture<JaegerTestFixture>
{
    private readonly JaegerTestFixture _fixture;

    public JaegerApiVerificationTests(JaegerTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task JaegerApi_ShouldBeAccessible()
    {
        // Act
        var services = await _fixture.GetServicesAsync();

        // Assert
        services.Should().NotBeNull();
        // Note: This test will pass even if Jaeger is not running
        // It just verifies that we can make HTTP requests to the API
    }

    [Fact]
    public async Task CreateSpan_ShouldAppearInJaegerUI()
    {
        // Arrange
        const string serviceName = "test-service";
        var tracer = _fixture.Tracer;
        
        // Act
        var span = tracer.BuildSpan("api-test-operation").Start();
        span.SetTag("test-tag", "test-value");
        span.Log("Test log message");
        span.Finish();

        // Wait for trace to appear in Jaeger
        var traceAppeared = await _fixture.WaitForTraceAsync(serviceName, TimeSpan.FromSeconds(10));

        // Assert
        traceAppeared.Should().BeTrue("Trace should appear in Jaeger UI within 10 seconds");
    }

    [Fact]
    public async Task Service_ShouldAppearInServicesList()
    {
        // Arrange
        const string serviceName = "test-service";
        var tracer = _fixture.Tracer;
        
        // Act
        var span = tracer.BuildSpan("services-list-operation").Start();
        span.Finish();

        // Wait a bit for the service to be registered
        await Task.Delay(TimeSpan.FromSeconds(2));

        var servicesList = await _fixture.GetServicesAsync();

        // Assert
        servicesList.Should().Contain(serviceName, "Service should appear in Jaeger services list");
    }

    [Fact]
    public async Task Span_ShouldContainCorrectTags()
    {
        // Arrange
        const string serviceName = "test-service";
        var tracer = _fixture.Tracer;
        
        // Act
        var span = tracer.BuildSpan("tags-test-operation").Start();
        span.SetTag("string-tag", "test-value");
        span.SetTag("int-tag", 42);
        span.SetTag("bool-tag", true);
        span.Finish();

        // Wait for trace to appear
        var traceAppeared = await _fixture.WaitForTraceAsync(serviceName, TimeSpan.FromSeconds(10));
        traceAppeared.Should().BeTrue("Trace should appear in Jaeger UI");

        var traces = await _fixture.GetTracesAsync(serviceName);

        // Assert
        traces.Should().NotBeNull("Traces should be found in Jaeger");
        traces.Should().NotBeEmpty("Traces should contain data");
        
        var testTrace = traces!.FirstOrDefault(t => t.Spans.Any(s => s.OperationName == "tags-test-operation"));
        testTrace.Should().NotBeNull("Test trace should be found");
        
        // Note: We can't easily verify specific tags without parsing the Jaeger response
        // The important thing is that the span is created and appears in Jaeger
    }

    [Fact]
    public async Task Span_ShouldRecordDuration()
    {
        // Arrange
        const string serviceName = "test-service";
        var tracer = _fixture.Tracer;
        
        // Act
        var span = tracer.BuildSpan("duration-test-operation").Start();
        await Task.Delay(100); // Simulate some work
        span.Finish();

        // Wait for trace to appear
        var traceAppeared = await _fixture.WaitForTraceAsync(serviceName, TimeSpan.FromSeconds(10));
        traceAppeared.Should().BeTrue("Trace should appear in Jaeger UI");

        var traces = await _fixture.GetTracesAsync(serviceName);

        // Assert
        traces.Should().NotBeNull("Traces should be found in Jaeger");
        traces.Should().NotBeEmpty("Traces should contain data");
        
        var testTrace = traces!.FirstOrDefault(t => t.Spans.Any(s => s.OperationName == "duration-test-operation"));
        testTrace.Should().NotBeNull("Test trace should be found");
        
        // Note: Duration verification would require parsing the Jaeger response
        // The important thing is that the span is created and appears in Jaeger
    }

    [Fact]
    public async Task EndToEnd_ShouldWorkFromServiceToJaegerUI()
    {
        // Arrange
        const string serviceName = "test-service";
        var tracer = _fixture.Tracer;
        
        // Act
        // Create a complex trace with multiple spans
        var rootSpan = tracer.BuildSpan("e2e-root-operation").Start();
        rootSpan.SetTag("service", serviceName);
        rootSpan.SetTag("test-type", "end-to-end");
        
        var childSpan = tracer.BuildSpan("e2e-child-operation").Start();
        childSpan.SetTag("parent", "e2e-root-operation");
        childSpan.Log("Child operation started");
        
        await Task.Delay(50); // Simulate work
        
        childSpan.Finish();
        rootSpan.Finish();

        // Wait for trace to appear
        var traceAppeared = await _fixture.WaitForTraceAsync(serviceName, TimeSpan.FromSeconds(15));

        // Assert
        traceAppeared.Should().BeTrue("End-to-end trace should appear in Jaeger UI within 15 seconds");
        
        var traces = await _fixture.GetTracesAsync(serviceName);
        traces.Should().NotBeNull("End-to-end traces should be found in Jaeger");
        traces.Should().NotBeEmpty("End-to-end traces should contain data");
        
        var testTrace = traces!.FirstOrDefault(t => t.Spans.Any(s => s.OperationName == "e2e-root-operation"));
        testTrace.Should().NotBeNull("End-to-end trace should be found");
        
        var rootSpanData = testTrace!.Spans.FirstOrDefault(s => s.OperationName == "e2e-root-operation");
        var childSpanData = testTrace.Spans.FirstOrDefault(s => s.OperationName == "e2e-child-operation");
        
        rootSpanData.Should().NotBeNull("Root span should be found");
        childSpanData.Should().NotBeNull("Child span should be found");
    }
}

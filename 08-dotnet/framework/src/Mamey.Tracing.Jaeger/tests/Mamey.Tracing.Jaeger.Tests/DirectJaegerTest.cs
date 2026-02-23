using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;
using Xunit;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders;
using Jaeger.Senders.Thrift;
using Mamey.Tracing.Jaeger.Tests.Helpers;

namespace Mamey.Tracing.Jaeger.Tests;

public class DirectJaegerTest : IClassFixture<JaegerTestFixture>
{
    private readonly JaegerTestFixture _fixture;

    public DirectJaegerTest(JaegerTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DirectJaegerTracer_ShouldSendTracesToJaeger()
    {
        // Arrange
        const string serviceName = "direct-test-service";
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        var serviceProvider = services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        
        // Create Jaeger tracer directly
        var sender = new UdpSender("localhost", 6831, 64967);
        var reporter = new RemoteReporter.Builder()
            .WithSender(sender)
            .WithLoggerFactory(loggerFactory)
            .Build();
        
        var sampler = new ConstSampler(true);
        
        var tracer = new Tracer.Builder(serviceName)
            .WithLoggerFactory(loggerFactory)
            .WithReporter(reporter)
            .WithSampler(sampler)
            .Build();
        
        // Act - Create spans
        for (int i = 0; i < 3; i++)
        {
            var span = tracer.BuildSpan($"direct-test-{i}").Start();
            span.SetTag("test-number", i);
            span.SetTag("service", serviceName);
            span.Log($"Direct test log {i}");
            span.Finish();
        }
        
        // Wait for traces to be sent
        await Task.Delay(3000);
        
        // Check if service appears in services list
        var servicesList = await _fixture.GetServicesAsync();
        servicesList.Should().Contain(serviceName, "Service should appear in Jaeger services list");
        
        // Check if traces appear
        var traces = await _fixture.GetTracesAsync(serviceName);
        traces.Should().NotBeNull("Traces should be found");
        traces.Should().NotBeEmpty("Traces should contain data");
        
        // Cleanup
        tracer.Dispose();
        serviceProvider.Dispose();
    }

    [Fact]
    public void DirectJaegerTracer_ShouldCreateSpansWithoutErrors()
    {
        // Arrange
        const string serviceName = "direct-test-service";
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        var serviceProvider = services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        
        // Create Jaeger tracer directly
        var sender = new UdpSender("localhost", 6831, 64967);
        var reporter = new RemoteReporter.Builder()
            .WithSender(sender)
            .WithLoggerFactory(loggerFactory)
            .Build();
        
        var sampler = new ConstSampler(true);
        
        var tracer = new Tracer.Builder(serviceName)
            .WithLoggerFactory(loggerFactory)
            .WithReporter(reporter)
            .WithSampler(sampler)
            .Build();
        
        // Act & Assert
        var action = () =>
        {
            var span = tracer.BuildSpan("direct-test").Start();
            span.SetTag("test", "value");
            span.Log("Test log");
            span.Finish();
        };
        
        action.Should().NotThrow("Direct Jaeger tracer should create spans without errors");
        
        // Cleanup
        tracer.Dispose();
        serviceProvider.Dispose();
    }
}

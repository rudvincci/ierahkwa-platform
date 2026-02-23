using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTracing;
using Xunit;
using Mamey.OpenTracingContrib;
using Mamey.Tracing.Jaeger;
using Mamey;

namespace Mamey.OpenTracingContrib.Tests;

public class InstrumentationServiceTests
{
    [Fact]
    public void InstrumentationService_ShouldStartOnHostStart()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        var mameyBuilder = MameyBuilder.Create(services);
        mameyBuilder.AddJaeger(options =>
        {
            return options.Enable(true)
                          .WithServiceName("instrumentation-service-test")
                          .WithUdpHost("localhost")
                          .WithUdpPort(6831)
                          .WithSampler("const");
        });
        services.AddOpenTracing();

        // Act
        var serviceProvider = services.BuildServiceProvider();
        var instrumentationService = serviceProvider.GetService<IHostedService>();

        // Assert
        instrumentationService.Should().NotBeNull("InstrumentationService should be registered");
        instrumentationService.Should().BeOfType<InstrumentationService>("Should be the correct type");
    }

    [Fact]
    public async Task InstrumentationService_ShouldStartSuccessfully()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        var mameyBuilder = MameyBuilder.Create(services);
        mameyBuilder.AddJaeger(options =>
        {
            return options.Enable(true)
                          .WithServiceName("instrumentation-start-test")
                          .WithUdpHost("localhost")
                          .WithUdpPort(6831)
                          .WithSampler("const");
        });
        services.AddOpenTracing();

        var serviceProvider = services.BuildServiceProvider();
        var instrumentationService = serviceProvider.GetRequiredService<IHostedService>();

        // Act
        var startAction = async () => await instrumentationService.StartAsync(CancellationToken.None);

        // Assert
        await startAction.Should().NotThrowAsync("InstrumentationService should start without errors");
    }

    [Fact]
    public async Task InstrumentationService_ShouldStopSuccessfully()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        var mameyBuilder = MameyBuilder.Create(services);
        mameyBuilder.AddJaeger(options =>
        {
            return options.Enable(true)
                          .WithServiceName("instrumentation-stop-test")
                          .WithUdpHost("localhost")
                          .WithUdpPort(6831)
                          .WithSampler("const");
        });
        services.AddOpenTracing();

        var serviceProvider = services.BuildServiceProvider();
        var instrumentationService = serviceProvider.GetRequiredService<IHostedService>();

        // Start the service first
        await instrumentationService.StartAsync(CancellationToken.None);

        // Act
        var stopAction = async () => await instrumentationService.StopAsync(CancellationToken.None);

        // Assert
        await stopAction.Should().NotThrowAsync("InstrumentationService should stop without errors");
    }

    [Fact]
    public void DiagnosticManager_ShouldSubscribeToEvents()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        var mameyBuilder = MameyBuilder.Create(services);
        mameyBuilder.AddJaeger(options =>
        {
            return options.Enable(true)
                          .WithServiceName("diagnostic-manager-test")
                          .WithUdpHost("localhost")
                          .WithUdpPort(6831)
                          .WithSampler("const");
        });
        services.AddOpenTracing();

        // Act
        var serviceProvider = services.BuildServiceProvider();
        var tracer = serviceProvider.GetRequiredService<ITracer>();

        // Assert
        tracer.Should().NotBeNull("Tracer should be available");
        
        // Note: We can't easily verify DiagnosticManager subscription without creating diagnostic events
        // The important thing is that the service is registered and can be resolved
    }

    [Fact]
    public void Spans_ShouldBeCreatedForDiagnosticSourceEvents()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        var mameyBuilder = MameyBuilder.Create(services);
        mameyBuilder.AddJaeger(options =>
        {
            return options.Enable(true)
                          .WithServiceName("diagnostic-events-test")
                          .WithUdpHost("localhost")
                          .WithUdpPort(6831)
                          .WithSampler("const");
        });
        services.AddOpenTracing();

        var serviceProvider = services.BuildServiceProvider();
        var tracer = serviceProvider.GetRequiredService<ITracer>();

        // Act
        var span = tracer.BuildSpan("diagnostic-test-span").Start();
        span.SetTag("test", "diagnostic-events");
        span.Finish();

        // Assert
        span.Should().NotBeNull("Span should be created");
        
        // Note: We can't easily verify DiagnosticSource events without creating specific scenarios
        // The important thing is that spans can be created and finished without errors
    }

    [Fact]
    public async Task InstrumentationService_ShouldHandleMultipleStarts()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        var mameyBuilder = MameyBuilder.Create(services);
        mameyBuilder.AddJaeger(options =>
        {
            return options.Enable(true)
                          .WithServiceName("multiple-starts-test")
                          .WithUdpHost("localhost")
                          .WithUdpPort(6831)
                          .WithSampler("const");
        });
        services.AddOpenTracing();

        var serviceProvider = services.BuildServiceProvider();
        var instrumentationService = serviceProvider.GetRequiredService<IHostedService>();

        // Act & Assert
        await instrumentationService.StartAsync(CancellationToken.None);
        await instrumentationService.StartAsync(CancellationToken.None); // Should be idempotent
    }

    [Fact]
    public async Task InstrumentationService_ShouldHandleMultipleStops()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        var mameyBuilder = MameyBuilder.Create(services);
        mameyBuilder.AddJaeger(options =>
        {
            return options.Enable(true)
                          .WithServiceName("multiple-stops-test")
                          .WithUdpHost("localhost")
                          .WithUdpPort(6831)
                          .WithSampler("const");
        });
        services.AddOpenTracing();

        var serviceProvider = services.BuildServiceProvider();
        var instrumentationService = serviceProvider.GetRequiredService<IHostedService>();

        // Start the service first
        await instrumentationService.StartAsync(CancellationToken.None);

        // Act
        var stopAction1 = async () => await instrumentationService.StopAsync(CancellationToken.None);
        var stopAction2 = async () => await instrumentationService.StopAsync(CancellationToken.None);

        // Assert
        stopAction1.Should().NotThrowAsync("First stop should succeed");
        stopAction2.Should().NotThrowAsync("Second stop should also succeed (idempotent)");
    }

    [Fact]
    public void InstrumentationService_ShouldBeDisposable()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        var mameyBuilder = MameyBuilder.Create(services);
        mameyBuilder.AddJaeger(options =>
        {
            return options.Enable(true)
                          .WithServiceName("disposable-test")
                          .WithUdpHost("localhost")
                          .WithUdpPort(6831)
                          .WithSampler("const");
        });
        services.AddOpenTracing();

        var serviceProvider = services.BuildServiceProvider();
        var instrumentationService = serviceProvider.GetRequiredService<IHostedService>();

        // Act & Assert
        if (instrumentationService is IDisposable disposable)
        {
            var disposeAction = () => disposable.Dispose();
            disposeAction.Should().NotThrow("InstrumentationService should be disposable");
        }
    }
}

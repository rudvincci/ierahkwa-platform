using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;
using Xunit;
using Mamey.OpenTracingContrib;
using Mamey;
using Mamey.Tracing.Jaeger;

namespace Mamey.OpenTracingContrib.Tests;

public class OpenTracingBuilderTests
{
    [Fact]
    public void AddOpenTracing_ShouldRegisterITracer()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        
        var mameyBuilder = MameyBuilder.Create(services);
        mameyBuilder.AddJaeger(options =>
        {
            return options.WithServiceName("opentracing-builder-test")
                          .WithUdpHost("localhost")
                          .WithUdpPort(6831)
                          .WithSampler("const");
        });

        // Act
        services.AddOpenTracing();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var tracer = serviceProvider.GetService<ITracer>();
        tracer.Should().NotBeNull("ITracer should be registered");
    }

    [Fact]
    public void AddOpenTracing_ShouldConfigureAspNetCoreDiagnostics()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        var mameyBuilder = MameyBuilder.Create(services);
        mameyBuilder.AddJaeger(options =>
        {
            return options.WithServiceName("aspnetcore-diagnostics-test")
                          .WithUdpHost("localhost")
                          .WithUdpPort(6831)
                          .WithSampler("const");
        });

        // Act
        services.AddOpenTracing(builder =>
        {
            builder.AddAspNetCore();
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var tracer = serviceProvider.GetService<ITracer>();
        tracer.Should().NotBeNull("ITracer should be registered");
        
        // Note: We can't easily verify AspNetCore diagnostics without creating an HTTP request
        // The important thing is that no exceptions are thrown during configuration
    }

    [Fact]
    public void AddOpenTracing_ShouldConfigureEntityFrameworkCoreDiagnostics()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        var mameyBuilder = MameyBuilder.Create(services);
        mameyBuilder.AddJaeger(options =>
        {
            return options.WithServiceName("efcore-diagnostics-test")
                          .WithUdpHost("localhost")
                          .WithUdpPort(6831)
                          .WithSampler("const");
        });

        // Act
        services.AddOpenTracing(builder =>
        {
            builder.AddEntityFrameworkCore();
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var tracer = serviceProvider.GetService<ITracer>();
        tracer.Should().NotBeNull("ITracer should be registered");
        
        // Note: We can't easily verify EF Core diagnostics without creating a DbContext
        // The important thing is that no exceptions are thrown during configuration
    }

    [Fact]
    public void AddOpenTracing_ShouldConfigureHttpHandlerDiagnostics()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        var mameyBuilder = MameyBuilder.Create(services);
        mameyBuilder.AddJaeger(options =>
        {
            return options.WithServiceName("http-handler-diagnostics-test")
                          .WithUdpHost("localhost")
                          .WithUdpPort(6831)
                          .WithSampler("const");
        });

        // Act
        services.AddOpenTracing(builder =>
        {
            builder.AddHttpHandler();
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var tracer = serviceProvider.GetService<ITracer>();
        tracer.Should().NotBeNull("ITracer should be registered");
        
        // Note: We can't easily verify HttpHandler diagnostics without making HTTP requests
        // The important thing is that no exceptions are thrown during configuration
    }

    [Fact]
    public void AddOpenTracing_ShouldConfigureAllDiagnosticsByDefault()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        var mameyBuilder = MameyBuilder.Create(services);
        mameyBuilder.AddJaeger(options =>
        {
            return options.WithServiceName("all-diagnostics-test")
                          .WithUdpHost("localhost")
                          .WithUdpPort(6831)
                          .WithSampler("const");
        });

        // Act
        services.AddOpenTracing(); // Should configure all diagnostics by default

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var tracer = serviceProvider.GetService<ITracer>();
        tracer.Should().NotBeNull("ITracer should be registered");
        
        // Note: We can't easily verify all diagnostics without creating various scenarios
        // The important thing is that no exceptions are thrown during configuration
    }

    [Fact]
    public void AddOpenTracing_ShouldHandleCustomBuilderConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        var mameyBuilder = MameyBuilder.Create(services);
        mameyBuilder.AddJaeger(options =>
        {
            return options.WithServiceName("custom-builder-test")
                          .WithUdpHost("localhost")
                          .WithUdpPort(6831)
                          .WithSampler("const");
        });

        // Act
        services.AddOpenTracing(builder =>
        {
            builder.AddLoggerProvider();
            builder.AddAspNetCore();
            // Don't add EF Core or HttpHandler to test custom configuration
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var tracer = serviceProvider.GetService<ITracer>();
        tracer.Should().NotBeNull("ITracer should be registered");
    }

    [Fact]
    public void AddOpenTracing_ShouldWorkWithNullBuilder()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        var mameyBuilder = MameyBuilder.Create(services);
        mameyBuilder.AddJaeger(options =>
        {
            return options.Enable(true)
                          .WithServiceName("null-builder-test")
                          .WithUdpHost("localhost")
                          .WithUdpPort(6831)
                          .WithSampler("const");
        });

        // Act
        services.AddOpenTracing(null); // Pass null builder

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var tracer = serviceProvider.GetService<ITracer>();
        tracer.Should().NotBeNull("ITracer should be registered even with null builder");
    }
}

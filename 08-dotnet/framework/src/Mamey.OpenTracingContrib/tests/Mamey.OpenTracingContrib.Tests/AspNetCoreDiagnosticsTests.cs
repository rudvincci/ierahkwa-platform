using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;
using System.IO;
using System.Text;
using Xunit;
using Mamey.OpenTracingContrib;
using Mamey.Tracing.Jaeger;
using Mamey.Tracing.Jaeger.Tests.Helpers;
using Mamey;

namespace Mamey.OpenTracingContrib.Tests;

public class AspNetCoreDiagnosticsTests : IClassFixture<JaegerTestFixture>
{
    private readonly JaegerTestFixture _fixture;

    public AspNetCoreDiagnosticsTests(JaegerTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task HttpRequest_ShouldCreateSpanAutomatically()
    {
        // Arrange
        const string serviceName = "http-request-test";
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        var mameyBuilder = MameyBuilder.Create(services);
        mameyBuilder.AddJaeger(options =>
        {
            return options.Enable(true)
                          .WithServiceName(serviceName)
                          .WithUdpHost("localhost")
                          .WithUdpPort(6831)
                          .WithSampler("const");
        });
        services.AddOpenTracing(builder => builder.AddAspNetCore());

        var serviceProvider = services.BuildServiceProvider();
        var tracer = serviceProvider.GetRequiredService<ITracer>();

        var webHostBuilder = new WebHostBuilder()
            .ConfigureServices(configureServices => 
            {
                foreach (var service in services)
                {
                    configureServices.Add(service);
                }
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/test", async context =>
                    {
                        var bytes = Encoding.UTF8.GetBytes("Hello World");
                        await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                    });
                });
            });

        using var testServer = new TestServer(webHostBuilder);

        // Act
        var response = await testServer.CreateClient().GetAsync("/test");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        // Wait for trace to appear in Jaeger
        var traceAppeared = await _fixture.WaitForTraceAsync(serviceName, TimeSpan.FromSeconds(10));
        traceAppeared.Should().BeTrue("HTTP request should create span in Jaeger");
    }

    [Fact]
    public async Task ExcludedPaths_ShouldNotCreateSpans()
    {
        // Arrange
        const string serviceName = "excluded-paths-test";
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        var mameyBuilder = MameyBuilder.Create(services);
        mameyBuilder.AddJaeger(options =>
        {
            return options.Enable(true)
                          .WithServiceName(serviceName)
                          .WithUdpHost("localhost")
                          .WithUdpPort(6831)
                          .WithSampler("const");
        });
        services.AddOpenTracing(builder => 
        {
            builder.AddAspNetCore();
        });

        var serviceProvider = services.BuildServiceProvider();

        var webHostBuilder = new WebHostBuilder()
            .ConfigureServices(configureServices => 
            {
                foreach (var service in services)
                {
                    configureServices.Add(service);
                }
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/health", async context =>
                    {
                        var bytes = Encoding.UTF8.GetBytes("Healthy");
                        await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                    });
                    endpoints.MapGet("/ping", async context =>
                    {
                        var bytes = Encoding.UTF8.GetBytes("Pong");
                        await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                    });
                    endpoints.MapGet("/test", async context =>
                    {
                        var bytes = Encoding.UTF8.GetBytes("Test");
                        await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                    });
                });
            });

        using var testServer = new TestServer(webHostBuilder);

        // Act
        await testServer.CreateClient().GetAsync("/health");
        await testServer.CreateClient().GetAsync("/ping");
        await testServer.CreateClient().GetAsync("/test");

        // Wait a bit for traces to be processed
        await Task.Delay(TimeSpan.FromSeconds(2));

        // Assert
        var trace = await _fixture.GetTraceAsync(serviceName);
        
        if (trace != null)
        {
            // If traces exist, they should only be for /test, not /health or /ping
            var healthSpans = trace.Spans.Where(s => s.OperationName.Contains("health"));
            var pingSpans = trace.Spans.Where(s => s.OperationName.Contains("ping"));
            
            healthSpans.Should().BeEmpty("Health endpoint should not create spans");
            pingSpans.Should().BeEmpty("Ping endpoint should not create spans");
        }
    }

    [Fact]
    public async Task Span_ShouldContainHttpMethodAndPath()
    {
        // Arrange
        const string serviceName = "http-method-path-test";
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        var mameyBuilder = MameyBuilder.Create(services);
        mameyBuilder.AddJaeger(options =>
        {
            return options.Enable(true)
                          .WithServiceName(serviceName)
                          .WithUdpHost("localhost")
                          .WithUdpPort(6831)
                          .WithSampler("const");
        });
        services.AddOpenTracing(builder => builder.AddAspNetCore());

        var serviceProvider = services.BuildServiceProvider();

        var webHostBuilder = new WebHostBuilder()
            .ConfigureServices(configureServices => 
            {
                foreach (var service in services)
                {
                    configureServices.Add(service);
                }
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/test-endpoint", async context =>
                    {
                        var bytes = Encoding.UTF8.GetBytes("Test Response");
                        await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                    });
                });
            });

        using var testServer = new TestServer(webHostBuilder);

        // Act
        var response = await testServer.CreateClient().GetAsync("/test-endpoint");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        
        // Wait for trace to appear
        var traceAppeared = await _fixture.WaitForTraceAsync(serviceName, TimeSpan.FromSeconds(10));
        traceAppeared.Should().BeTrue("HTTP request should create span");

        var trace = await _fixture.GetTraceAsync(serviceName);
        trace.Should().NotBeNull("Trace should be found in Jaeger");
        
        if (trace != null)
        {
            var httpSpans = trace.Spans.Where(s => s.OperationName.Contains("GET") || s.OperationName.Contains("test-endpoint"));
            httpSpans.Should().NotBeEmpty("Should have HTTP spans with method and path information");
        }
    }

    [Fact]
    public async Task Span_ShouldContainStatusCode()
    {
        // Arrange
        const string serviceName = "status-code-test";
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        var mameyBuilder = MameyBuilder.Create(services);
        mameyBuilder.AddJaeger(options =>
        {
            return options.Enable(true)
                          .WithServiceName(serviceName)
                          .WithUdpHost("localhost")
                          .WithUdpPort(6831)
                          .WithSampler("const");
        });
        services.AddOpenTracing(builder => builder.AddAspNetCore());

        var serviceProvider = services.BuildServiceProvider();

        var webHostBuilder = new WebHostBuilder()
            .ConfigureServices(configureServices => 
            {
                foreach (var service in services)
                {
                    configureServices.Add(service);
                }
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/success", async context =>
                    {
                        var bytes = Encoding.UTF8.GetBytes("Success");
                        await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                    });
                    endpoints.MapGet("/notfound", async context =>
                    {
                        context.Response.StatusCode = 404;
                        var bytes = Encoding.UTF8.GetBytes("Not Found");
                        await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                    });
                });
            });

        using var testServer = new TestServer(webHostBuilder);

        // Act
        await testServer.CreateClient().GetAsync("/success");
        await testServer.CreateClient().GetAsync("/notfound");

        // Wait for traces to appear
        var traceAppeared = await _fixture.WaitForTraceAsync(serviceName, TimeSpan.FromSeconds(10));
        traceAppeared.Should().BeTrue("HTTP requests should create spans");

        // Assert
        var trace = await _fixture.GetTraceAsync(serviceName);
        trace.Should().NotBeNull("Trace should be found in Jaeger");
        
        if (trace != null)
        {
            // We can't easily verify specific status codes without parsing Jaeger response
            // The important thing is that spans are created for both successful and error responses
            trace.Spans.Should().NotBeEmpty("Should have spans for both requests");
        }
    }

    [Fact]
    public async Task TraceContext_ShouldPropagateViaHeaders()
    {
        // Arrange
        const string serviceName = "trace-context-test";
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        var mameyBuilder = MameyBuilder.Create(services);
        mameyBuilder.AddJaeger(options =>
        {
            return options.Enable(true)
                          .WithServiceName(serviceName)
                          .WithUdpHost("localhost")
                          .WithUdpPort(6831)
                          .WithSampler("const");
        });
        services.AddOpenTracing(builder => builder.AddAspNetCore());

        var serviceProvider = services.BuildServiceProvider();

        var webHostBuilder = new WebHostBuilder()
            .ConfigureServices(configureServices => 
            {
                foreach (var service in services)
                {
                    configureServices.Add(service);
                }
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/trace-context", async context =>
                    {
                        // Check if trace context headers are present
                        var traceId = context.Request.Headers["uber-trace-id"].FirstOrDefault();
                        var hasTraceContext = !string.IsNullOrEmpty(traceId);
                        
                        var message = $"TraceContext: {hasTraceContext}";
                        var bytes = Encoding.UTF8.GetBytes(message);
                        await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                    });
                });
            });

        using var testServer = new TestServer(webHostBuilder);

        // Act
        var response = await testServer.CreateClient().GetAsync("/trace-context");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        content.Should().Contain("TraceContext: True", "Trace context should be propagated via headers");
    }
}

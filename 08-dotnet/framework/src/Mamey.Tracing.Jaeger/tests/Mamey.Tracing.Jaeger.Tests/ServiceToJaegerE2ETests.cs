using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;
using Xunit;
using Mamey.Tracing.Jaeger;
using Mamey.OpenTracingContrib;
using Mamey.Tracing.Jaeger.Tests.Helpers;
using Mamey;
using System.Net.Http.Json;

namespace Mamey.Tracing.Jaeger.Tests;

public class ServiceToJaegerE2ETests : IClassFixture<JaegerTestFixture>
{
    private readonly JaegerTestFixture _fixture;

    public ServiceToJaegerE2ETests(JaegerTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task TestTraceEndpoint_ShouldAppearInJaegerUI()
    {
        // Arrange
        const string serviceName = "fwid-identity-service";
        var webHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddLogging(builder => builder.AddConsole());
                
                var mameyBuilder = MameyBuilder.Create(services);
                mameyBuilder.AddJaeger(options =>
                {
                    return options.Enable(true)
                                  .WithServiceName(serviceName)
                                  .WithSampler("const")
                                  .WithSamplingRate(1.0);
                });
                services.AddOpenTracing(builder => builder.AddAspNetCore());
                services.AddRouting();
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/test-trace", async context =>
                    {
                        var tracer = context.RequestServices.GetRequiredService<ITracer>();
                        var span = tracer.BuildSpan("test-trace").Start();
                        span.SetTag("service", serviceName);
                        span.SetTag("endpoint", "/test-trace");
                        span.Log("Test trace endpoint called");
                        
                        await Task.Delay(100); // Simulate some work
                        
                        span.Finish();
                        await context.Response.WriteAsync("Test trace completed");
                    });
                });
            });

        using var testServer = new TestServer(webHostBuilder);

        // Act
        var response = await testServer.CreateClient().GetAsync("/test-trace");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue("Test trace endpoint should respond successfully");
        content.Should().Contain("Test trace completed", "Should return expected response");

        // Wait for trace to appear in Jaeger UI
        var traceAppeared = await WaitForTraceAsync(serviceName, TimeSpan.FromSeconds(15));
        traceAppeared.Should().BeTrue("Trace should appear in Jaeger UI within 15 seconds");
    }

    [Fact]
    public async Task Span_ShouldContainExpectedTags()
    {
        // Arrange
        const string serviceName = "tags-verification-e2e-test";
        var webHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddLogging(builder => builder.AddConsole());
                
                var mameyBuilder = MameyBuilder.Create(services);
                mameyBuilder.AddJaeger(options =>
                {
                    return options.Enable(true)
                                  .WithServiceName(serviceName)
                                  .WithSampler("const")
                                  .WithSamplingRate(1.0);
                });
                services.AddOpenTracing(builder => builder.AddAspNetCore());
                services.AddRouting();
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/tags-test", async context =>
                    {
                        var tracer = context.RequestServices.GetRequiredService<ITracer>();
                        var span = tracer.BuildSpan("tags-test-operation").Start();
                        span.SetTag("service", serviceName);
                        span.SetTag("endpoint", "/tags-test");
                        span.SetTag("http.method", "GET");
                        span.SetTag("http.status_code", 200);
                        span.SetTag("test.type", "end-to-end");
                        span.Finish();
                        await context.Response.WriteAsync("Tags test completed");
                    });
                });
            });

        using var testServer = new TestServer(webHostBuilder);

        // Act
        var response = await testServer.CreateClient().GetAsync("/tags-test");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        // Wait for trace to appear
        var traceAppeared = await WaitForTraceAsync(serviceName, TimeSpan.FromSeconds(15));
        traceAppeared.Should().BeTrue("Trace should appear in Jaeger UI");

        var trace = await GetTraceAsync(serviceName);
        trace.Should().NotBeNull("Trace should be found in Jaeger");
        
        if (trace != null)
        {
            var span = trace.Spans.FirstOrDefault(s => s.OperationName == "tags-test-operation");
            span.Should().NotBeNull("Tags test span should be found");
        }
    }

    [Fact]
    public async Task Span_ShouldShowCorrectOperationName()
    {
        // Arrange
        const string serviceName = "operation-name-e2e-test";
        const string expectedOperationName = "test-trace";
        var webHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddLogging(builder => builder.AddConsole());
                
                var mameyBuilder = MameyBuilder.Create(services);
                mameyBuilder.AddJaeger(options =>
                {
                    return options.Enable(true)
                                  .WithServiceName(serviceName)
                                  .WithSampler("const")
                                  .WithSamplingRate(1.0);
                });
                services.AddOpenTracing(builder => builder.AddAspNetCore());
                services.AddRouting();
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/operation-test", async context =>
                    {
                        var tracer = context.RequestServices.GetRequiredService<ITracer>();
                        var span = tracer.BuildSpan(expectedOperationName).Start();
                        span.Finish();
                        await context.Response.WriteAsync("Operation test completed");
                    });
                });
            });

        using var testServer = new TestServer(webHostBuilder);

        // Act
        var response = await testServer.CreateClient().GetAsync("/operation-test");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        // Wait for trace to appear
        var traceAppeared = await WaitForTraceAsync(serviceName, TimeSpan.FromSeconds(15));
        traceAppeared.Should().BeTrue("Trace should appear in Jaeger UI");

        var trace = await GetTraceAsync(serviceName);
        trace.Should().NotBeNull("Trace should be found in Jaeger");
        
        if (trace != null)
        {
            var span = trace.Spans.FirstOrDefault(s => s.OperationName == expectedOperationName);
            span.Should().NotBeNull($"Span with operation name '{expectedOperationName}' should be found");
        }
    }

    [Fact]
    public async Task MultipleRequests_ShouldCreateMultipleTraces()
    {
        // Arrange
        const string serviceName = "multiple-traces-e2e-test";
        var webHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddLogging(builder => builder.AddConsole());
                
                var mameyBuilder = MameyBuilder.Create(services);
                mameyBuilder.AddJaeger(options =>
                {
                    return options.Enable(true)
                                  .WithServiceName(serviceName)
                                  .WithSampler("const")
                                  .WithSamplingRate(1.0);
                });
                services.AddOpenTracing(builder => builder.AddAspNetCore());
                services.AddRouting();
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/multi-test", async context =>
                    {
                        var tracer = context.RequestServices.GetRequiredService<ITracer>();
                        var span = tracer.BuildSpan("multi-test-operation").Start();
                        span.SetTag("request.id", Guid.NewGuid().ToString());
                        span.Finish();
                        await context.Response.WriteAsync("Multi test completed");
                    });
                });
            });

        using var testServer = new TestServer(webHostBuilder);

        // Act
        var tasks = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 3; i++)
        {
            tasks.Add(testServer.CreateClient().GetAsync("/multi-test"));
        }
        
        var responses = await Task.WhenAll(tasks);

        // Assert
        responses.Should().AllSatisfy(r => r.IsSuccessStatusCode.Should().BeTrue());

        // Wait for traces to appear
        var traceAppeared = await WaitForTraceAsync(serviceName, TimeSpan.FromSeconds(20));
        traceAppeared.Should().BeTrue("Traces should appear in Jaeger UI");

        var trace = await GetTraceAsync(serviceName);
        trace.Should().NotBeNull("Trace should be found in Jaeger");
        
        if (trace != null)
        {
            var spans = trace.Spans.Where(s => s.OperationName == "multi-test-operation");
            spans.Should().NotBeEmpty("Should have multiple spans for multiple requests");
        }
    }

    [Fact]
    public async Task FullIntegration_ShouldWorkFromServiceToJaegerUI()
    {
        // Arrange
        const string serviceName = "full-integration-e2e-test";
        var webHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddLogging(builder => builder.AddConsole());
                
                var mameyBuilder = MameyBuilder.Create(services);
                mameyBuilder.AddJaeger(options =>
                {
                    return options.Enable(true)
                                  .WithServiceName(serviceName)
                                  .WithSampler("const")
                                  .WithSamplingRate(1.0);
                });
                services.AddOpenTracing(builder => builder.AddAspNetCore());
                services.AddRouting();
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/full-integration", async context =>
                    {
                        var tracer = context.RequestServices.GetRequiredService<ITracer>();
                        
                        // Create a complex trace with multiple spans
                        var rootSpan = tracer.BuildSpan("full-integration-root").Start();
                        rootSpan.SetTag("service", serviceName);
                        rootSpan.SetTag("test.type", "full-integration");
                        rootSpan.Log("Starting full integration test");
                        
                        var childSpan = tracer.BuildSpan("full-integration-child").Start();
                        childSpan.SetTag("parent", "full-integration-root");
                        childSpan.Log("Child operation started");
                        
                        await Task.Delay(50); // Simulate work
                        
                        childSpan.Finish();
                        rootSpan.Log("Child operation completed");
                        rootSpan.Finish();
                        
                        await context.Response.WriteAsync("Full integration test completed");
                    });
                });
            });

        using var testServer = new TestServer(webHostBuilder);

        // Act
        var response = await testServer.CreateClient().GetAsync("/full-integration");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue("Full integration endpoint should respond successfully");
        content.Should().Contain("Full integration test completed", "Should return expected response");

        // Wait for trace to appear in Jaeger UI
        var traceAppeared = await WaitForTraceAsync(serviceName, TimeSpan.FromSeconds(20));
        traceAppeared.Should().BeTrue("Full integration trace should appear in Jaeger UI within 20 seconds");
        
        var trace = await GetTraceAsync(serviceName);
        trace.Should().NotBeNull("Full integration trace should be found in Jaeger");
        
        if (trace != null)
        {
            trace.Spans.Should().HaveCount(2, "Full integration trace should contain 2 spans (root and child)");
            
            var rootSpan = trace.Spans.FirstOrDefault(s => s.OperationName == "full-integration-root");
            var childSpan = trace.Spans.FirstOrDefault(s => s.OperationName == "full-integration-child");
            
            rootSpan.Should().NotBeNull("Root span should be found");
            childSpan.Should().NotBeNull("Child span should be found");
        }
    }

    private async Task<bool> WaitForTraceAsync(string serviceName, TimeSpan timeout)
    {
        var startTime = DateTime.UtcNow;
        while (DateTime.UtcNow - startTime < timeout)
        {
            try
            {
                var response = await _fixture.JaegerApiClient.GetFromJsonAsync<JaegerTracesResponse>(
                    $"/api/traces?service={serviceName}&limit=20");
                
                if (response?.Data?.Any() == true)
                {
                    return true;
                }
            }
            catch (HttpRequestException)
            {
                // Jaeger might not be ready yet
            }

            await Task.Delay(TimeSpan.FromMilliseconds(500));
        }

        return false;
    }

    private async Task<JaegerTrace?> GetTraceAsync(string serviceName)
    {
        try
        {
            var response = await _fixture.JaegerApiClient.GetFromJsonAsync<JaegerTracesResponse>(
                $"/api/traces?service={serviceName}&limit=20");
            
            return response?.Data?.FirstOrDefault();
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }
}
